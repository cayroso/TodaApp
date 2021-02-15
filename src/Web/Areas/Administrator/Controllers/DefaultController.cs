using App.CQRS;
using Data.App.DbContext;
using Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                .Include(e => e.Driver).ThenInclude(e => e.User)
                .Include(e => e.Rider).ThenInclude(e => e.User)
                .AsNoTracking().ToListAsync();
            var totalCompletedTrips = trips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete);

            var totalTripsCount = trips.Count();
            var totalComplatedTripCount = totalCompletedTrips.Count();

            var totalEarnings = totalCompletedTrips.Sum(e => e.Fare);

            // top drivers
            var topDrivers = totalCompletedTrips.GroupBy(e => e.Driver.User.FirstLastName)
                            .Select(e => new
                            {
                                Rider = e.Key,
                                TotalFare = e.Sum(p => p.Fare),
                                TotalTrip = e.Count()
                            }).OrderByDescending(e => e.TotalFare).Take(10).ToList();

            // top riders
            var topRiders = totalCompletedTrips.GroupBy(e => e.Rider.User.FirstLastName)
                            .Select(e => new
                            {
                                Rider = e.Key,
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
