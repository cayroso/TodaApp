using App.CQRS.Trips.Common.Commands.Command;
using App.CQRS.Trips.Common.Commands.Command.Driver;
using App.CQRS.Trips.Common.Commands.Command.Rider;
using App.Hubs;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Notifications;
using Data.App.Models.Trips;
using Data.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Handler
{
    public sealed class TripCommonCommandHandler :
        ICommandHandler<DriverAcceptRiderTripRequestCommand>,
        ICommandHandler<DriverOfferFareToRiderTripRequestCommand>,
        ICommandHandler<DriverRejectRiderTripRequestCommand>,
        ICommandHandler<RiderAcceptDriverOfferCommand>,
        ICommandHandler<RiderCancelTripCommand>,
        ICommandHandler<RiderCreateTripCommand>,
        ICommandHandler<RiderRejectDriverOfferCommand>,
        ICommandHandler<RiderRequestTripCommand>,
        ICommandHandler<SetTripToCompleteCommand>,
        ICommandHandler<SetTripToInProgressCommand>

    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        readonly NotificationService _notificationService;
        readonly IHubContext<TripHub, ITripClient> _tripHubContext;
        public TripCommonCommandHandler(
            AppDbContext appDbContext,
            ISequentialGuidGenerator sequentialGuidGenerator,
            IHubContext<TripHub, ITripClient> tripHubContext,
            NotificationService notificationService)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
            _tripHubContext = tripHubContext ?? throw new ArgumentNullException(nameof(tripHubContext));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        #region Driver

        async Task ICommandHandler<DriverAcceptRiderTripRequestCommand>.HandleAsync(DriverAcceptRiderTripRequestCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.DriverAccepted;
            data.Fare = 0;
            data.Driver.Availability = EnumDriverAvailability.Unavailable;

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.RiderId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                "Driver Accepted", "The assigned driver accepted your trip request.", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).DriverAccepted(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
            });
        }

        async Task ICommandHandler<DriverOfferFareToRiderTripRequestCommand>.HandleAsync(DriverOfferFareToRiderTripRequestCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.DriverOfferedFare;
            data.Fare = command.Fare;

            var notes = $"Offered Fare: {command.Fare}" + Environment.NewLine + command.Notes;

            data.AddTimeline(command.UserId, data.Status, notes);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.RiderId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                "Driver Offered Fare", $"The assigned drive offered {command.Fare} fare to your trip request.", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).DriverFareOffered(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
                Fare = command.Fare
            });
        }

        async Task ICommandHandler<DriverRejectRiderTripRequestCommand>.HandleAsync(DriverRejectRiderTripRequestCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.DriverRejected;
            data.Fare = 0;
            data.Driver.Availability = EnumDriverAvailability.Available;

            data.AddTimeline(command.UserId, data.Status, command.Notes);

            data.ExcludedDrivers.Add(new TripExcludedDriver
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                Reason = command.Notes,
            });

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.RiderId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-exclamation-circle",
                "Driver Rejected", "The assigned driver rejected your trip request." + Environment.NewLine + $"With reason: {command.Notes}", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).DriverRejected(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
            });
        }


        #endregion

        #region Rider

        async Task ICommandHandler<RiderAcceptDriverOfferCommand>.HandleAsync(RiderAcceptDriverOfferCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.RiderOfferedFareAccepted;
            data.Driver.Availability = EnumDriverAvailability.Unavailable;

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.DriverId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                "Rider Accepted Offered Fare", $"The rider accepted your offered fare {data.Fare}.", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).RiderOfferedFareAccepted(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
                Fare = data.Fare
            });
        }

        async Task ICommandHandler<RiderCancelTripCommand>.HandleAsync(RiderCancelTripCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.Cancelled;

            if (data.Driver != null)
            {
                data.Driver.Availability = EnumDriverAvailability.Available;
            }

            data.AddTimeline(command.UserId, data.Status, command.Notes);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.DriverId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-exclamation-circle",
                "Rider Cancelled", "The rider cancelled the trip." + Environment.NewLine + $"With reason: {command.Notes}", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).RiderTripCancelled(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
                Reason = command.Notes
            });
        }

        async Task ICommandHandler<RiderCreateTripCommand>.HandleAsync(RiderCreateTripCommand command)
        {
            var existingTrip = await _appDbContext.Trips.Where(e => e.RiderId == command.UserId
                        && !(e.Status == EnumTripStatus.Complete || e.Status == EnumTripStatus.Cancelled)).AnyAsync();

            if (existingTrip)
            {
                throw new ApplicationException("You have active trip.");
            }

            var data = new Trip
            {
                TripId = command.TripId,
                RiderId = command.RiderId,

                StartAddress = command.StartAddress,
                StartAddressDescription = command.StartAddressDescription,
                StartX = command.StartX,
                StartY = command.StartY,

                EndAddress = command.EndAddress,
                EndAddressDescription = command.EndAddressDescription,
                EndX = command.EndX,
                EndY = command.EndY,

                Status = EnumTripStatus.Pending,
            };

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.AddAsync(data);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderRejectDriverOfferCommand>.HandleAsync(RiderRejectDriverOfferCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            var fare = data.Fare;

            data.Status = EnumTripStatus.RiderOfferedFareRejected;
            data.Fare = 0;
            data.Driver.Availability = EnumDriverAvailability.Available;

            data.AddTimeline(command.UserId, data.Status, command.Notes);

            data.ExcludedDrivers.Add(new TripExcludedDriver
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                Reason = $"Rider Rejected Offered Fare: {command.Notes}",
            });

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.DriverId };

            await _notificationService.DeleteNotificationByReferenceId(data.TripId);

            await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-exclamation-circle",
                "Rider Rejected Offered Fare", $"The rider rejected your offered fare {fare}." + Environment.NewLine + $"With reason: {command.Notes}", EnumNotificationType.Info, notifyIds, null);

            await _tripHubContext.Clients.Users(notifyIds).RiderOfferedFareRejected(new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
                Fare = fare
            });

        }

        async Task ICommandHandler<RiderRequestTripCommand>.HandleAsync(RiderRequestTripCommand command)
        {
            var data = await _appDbContext.Trips
                //.Include(e => e.Driver).ThenInclude(e => e.User)
                //.Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.Requested;
            data.Fare = 0;

            if (data.Driver != null)
            {
                data.Driver.Availability = EnumDriverAvailability.Available;
                data.DriverId = null;
            }

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<SetTripToCompleteCommand>.HandleAsync(SetTripToCompleteCommand command)
        {

            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.Complete;
            data.Driver.Availability = EnumDriverAvailability.Available;

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.RiderId };

            var response = new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
            };

            if (command.RiderInitiated)
            {
                notifyIds = new[] { data.DriverId };

                await _notificationService.DeleteNotificationByReferenceId(data.TripId);

                await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                    "Rider Completed", "The rider completed the trip. Please review the rider.", EnumNotificationType.Info, notifyIds, null);

                await _tripHubContext.Clients.Users(notifyIds).RiderTripCompleted(response);
            }
            else
            {
                await _notificationService.DeleteNotificationByReferenceId(data.TripId);

                await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                    "Driver Completed", "The rider completed the trip. Please review the driver.", EnumNotificationType.Info, notifyIds, null);

                await _tripHubContext.Clients.Users(notifyIds).DriverTripCompleted(response);
            }
        }

        async Task ICommandHandler<SetTripToInProgressCommand>.HandleAsync(SetTripToInProgressCommand command)
        {
            var data = await _appDbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .FirstOrDefaultAsync(e => e.TripId == command.TripId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.Status = EnumTripStatus.InProgress;
            data.Driver.Availability = EnumDriverAvailability.Unavailable;

            data.AddTimeline(command.UserId, data.Status, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.RiderId };

            var response = new Response
            {
                TripId = data.TripId,
                DriverId = data.DriverId,
                DriverName = data.Driver.User.FirstLastName,

                RiderId = data.Rider.RiderId,
                RiderName = data.Rider.User.FirstLastName,
            };

            if (command.RiderInitiated)
            {
                notifyIds = new[] { data.DriverId };

                await _notificationService.DeleteNotificationByReferenceId(data.TripId);

                await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                    "Rider Trip In-Progress", "The rider set the trip to in progress.", EnumNotificationType.Info, notifyIds, null);

                await _tripHubContext.Clients.Users(notifyIds).RiderTripInProgress(response);

            }
            else
            {
                await _notificationService.DeleteNotificationByReferenceId(data.TripId);

                await _notificationService.AddNotification(data.TripId, "fas fa-fw fa-info-circle",
                    "Driver Trip In-Progress", "The driver set the trip to in progress.", EnumNotificationType.Info, notifyIds, null);

                await _tripHubContext.Clients.Users(notifyIds).DriverTripInProgress(response);
            }
        }



        #endregion

    }
}
