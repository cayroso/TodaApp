using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Controllers;

namespace Web.Areas.Administrator.Controllers
{
    [Authorize(Policy = ApplicationRoles.AdministratorRoleName)]
    [ApiController]
    [Route("api/administrators/[controller]")]
    [Produces("application/json")]
    public class DefaultController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public DefaultController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromServices] AppDbContext appdbContext, int year, int month)
        {
            var users = await appdbContext.Users.ToListAsync();

            var drivers = await appdbContext.Drivers.AsNoTracking().CountAsync();
            var riders = await appdbContext.Riders.AsNoTracking().CountAsync();

            var trips = await appdbContext.Trips
                .Include(e => e.Driver).ThenInclude(e => e.User).ThenInclude(e => e.Image)
                .Include(e => e.Rider).ThenInclude(e => e.User).ThenInclude(e => e.Image)
                .AsNoTracking().ToListAsync();
            var totalCompletedTrips = trips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete);

            var totalTripsCount = trips.Count();
            var totalComplatedTripCount = totalCompletedTrips.Count();

            var totalEarnings = totalCompletedTrips.Sum(e => e.Fare);

            // top drivers
            var topDrivers = totalCompletedTrips.GroupBy(e => new { e.Driver.User.FirstLastName, e.Driver.OverallRating, e.Driver.User.Image?.Url })
                            .Select(e => new
                            {
                                Name = e.Key.FirstLastName,
                                ImageUrl = e.Key.Url,
                                Rating = e.Key.OverallRating,
                                TotalFare = e.Sum(p => p.Fare),
                                TotalTrip = e.Count()
                            }).OrderByDescending(e => e.TotalFare).Take(10).ToList();

            // top riders
            var topRiders = totalCompletedTrips.GroupBy(e => new { e.Rider.User.FirstLastName, e.Rider.OverallRating, e.Rider.User.Image?.Url })
                            .Select(e => new
                            {
                                Name = e.Key.FirstLastName,
                                ImageUrl = e.Key.Url,
                                Rating = e.Key.OverallRating,
                                TotalFare = e.Sum(p => p.Fare),
                                TotalTrip = e.Count()
                            }).OrderByDescending(e => e.TotalFare).Take(10).ToList();

            var dto = new
            {
                drivers,
                riders,
                totalTripsCount,
                totalComplatedTripCount,
                totalEarnings,
                topDrivers,
                topRiders,

            };



            return Ok(dto);
        }

        public class DashboardView
        {

        }
    }
}
