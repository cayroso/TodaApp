using App.Hubs;
using App.Services;
using Cayent.Core.CQRS.Services;
using Cayent.Core.Data.Notifications;
using Data.App.DbContext;
using Data.App.Models.Trips;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.BackgroundServices
{
    public class JobQueue<T>
    {
        private readonly ConcurrentQueue<T> _jobs = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(T job)
        {
            _jobs.Enqueue(job);
            _signal.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _signal.WaitAsync(cancellationToken);
            _jobs.TryDequeue(out var job);
            return job;
        }
    }

    public class MyJobBackgroundService : BackgroundService
    {
        private readonly ILogger<MyJobBackgroundService> _logger;
        private readonly JobQueue<Trip> _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MyJobBackgroundService(ILogger<MyJobBackgroundService> logger, JobQueue<Trip> queue, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var notifService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                        var trip = await appDbContext.Trips
                            .Include(e => e.Rider)
                                .ThenInclude(e => e.User)
                            .Include(e => e.ExcludedDrivers)
                            .FirstOrDefaultAsync(e => e.TripId == job.TripId);

                        //  get available drivers
                        var drivers = await appDbContext.Drivers.Include(e => e.User)
                            .Where(e => e.DriverId != trip.RiderId && e.Availability == Data.Enums.EnumDriverAvailability.Available)
                            .ToListAsync();

                        var filteredDrivers = drivers.Where(e => !trip.ExcludedDrivers.Any(ed => ed.DriverId == e.DriverId)).ToList();

                        if (trip != null && filteredDrivers.Any())
                        {
                            // do stuff
                            _logger.LogInformation("Working on job {JobId}", job.TripId);

                            //  TODO: sort drivers by rider's preferrence
                            var driver = filteredDrivers.First();

                            trip.DriverId = driver.DriverId;
                            trip.Status = Data.Enums.EnumTripStatus.DriverAssigned;
                            trip.Timelines.Add(new TripTimeline
                            {
                                TripId = trip.TripId,
                                UserId = trip.RiderId,
                                Status = trip.Status,
                            });

                            driver.Availability = Data.Enums.EnumDriverAvailability.Unavailable;

                            var tripContext = scope.ServiceProvider.GetRequiredService<IHubContext<TripHub, ITripClient>>();

                            //  TODO: notify driver/rider that trip is assigned
                            var response = new Response
                            {
                                TripId = trip.TripId,
                                DriverId = trip.DriverId,
                                DriverName = driver.User.FirstLastName,

                                RiderId = trip.RiderId,
                                RiderName = trip.Rider.User.FirstLastName
                            };

                            await tripContext.Clients.Users(new[] { trip.DriverId, trip.RiderId }).DriverAssigned(response);

                            await appDbContext.SaveChangesAsync();

                            var notifyIds = new[] { trip.DriverId, trip.RiderId };

                            await notifService.DeleteNotificationByReferenceId(trip.TripId);

                            await notifService.AddNotification(trip.TripId, "fas fa-fw fa-info-circle",
                                "Driver Assigned", "A driver was assigned to your trip request", EnumNotificationType.Info, new[] { trip.RiderId }, null);

                            await notifService.AddNotification(trip.TripId, "fas fa-fw fa-info-circle",
                                "Driver Assigned", "You was assigned as a Driver to a trip request", EnumNotificationType.Info, new[] { trip.DriverId }, null);
                        }
                        else
                        {
                            //  no available drivers, put them back
                            if (!filteredDrivers.Any())
                            {
                                _queue.Enqueue(job);
                                Console.WriteLine($"{DateTime.UtcNow} MyJobBackgroundService: No available driver found!!!");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //throw;
                }

                await Task.Delay(2000);
            }
        }
    }
}
