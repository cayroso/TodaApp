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

namespace Web.Areas.Driver.Controllers
{
    [Authorize(Policy = ApplicationRoles.DriverRoleName)]
    [ApiController]
    [Route("api/drivers/[controller]")]
    [Produces("application/json")]
    public class DefaultController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public DefaultController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromServices] AppDbContext appDbContext, int year, int month)
        {
            var now = DateTime.UtcNow;

            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = startMonth.AddMonths(1);

            var startWeek = now.AddDays(-(int)now.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var startLastWeek = startWeek.AddDays(-7);
            var endLastWeek = startWeek;

            var startToday = now.Date;
            var endToday = startToday.AddDays(1);

            var startYesterday = now.Date.AddDays(-1);
            var endYesterday = now.Date;

            var monthTrips = await appDbContext.Trips.Include(e => e.Rider).ThenInclude(e => e.User).AsNoTracking()
                .Where(e => e.DriverId == UserId && e.DateCreated >= startMonth && e.DateCreated <= endMonth).ToListAsync();

            var weekTrips = monthTrips.Where(e => e.DateCreated >= startWeek && e.DateCreated <= endWeek).ToList();

            var lastWeekTrips = monthTrips.Where(e => e.DateCreated >= startLastWeek && e.DateCreated <= endLastWeek).ToList();

            var todayTrips = monthTrips.Where(e => e.DateCreated >= startToday && e.DateCreated <= endToday).ToList();

            var yesterdayTrips = monthTrips.Where(e => e.DateCreated >= startYesterday && e.DateCreated <= endYesterday).ToList();

            //  no of trips month
            var totalMonthTrips = monthTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Count();

            //  total earning this month
            var sumMonthEarnings = monthTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Sum(e => e.Fare);

            // no of trips this week
            var totalWeekTrips = weekTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Count();

            // total earnings this week
            var sumWeekEarnings = weekTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Sum(e => e.Fare);

            //  no of trips last week
            var totalLastWeekTrips = lastWeekTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Count();

            //  sum of earnings last week
            var sumLastWeekEarnings = lastWeekTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Sum(e => e.Fare);

            //  no of trips today
            var totalTodayTrips = todayTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Count();

            //  sum of earnings today
            var sumTodayEarnings = todayTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Sum(e => e.Fare);

            //  no of trips yesterday
            var totalYesterdayTrips = yesterdayTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Count();

            //  sum of earnings yesterday
            var sumYesterdayEarnings = yesterdayTrips.Where(e => e.Status == Data.Enums.EnumTripStatus.Complete).Sum(e => e.Fare);

            // top riders
            var topRiders = monthTrips.GroupBy(e => e.Rider.User.FirstLastName)
                            .Select(e => new
                            {
                                Rider = e.Key,
                                TotalFare = e.Sum(p => p.Fare),
                                TotalTrip = e.Count()
                            }).OrderByDescending(e => e.TotalFare).ToList();

            var dto = new
            {
                totalMonthTrips,
                sumMonthEarnings,
                totalWeekTrips,
                sumWeekEarnings,
                totalLastWeekTrips,
                sumLastWeekEarnings,
                totalTodayTrips,
                sumTodayEarnings,
                totalYesterdayTrips,
                sumYesterdayEarnings,
                topRiders
            };

            return Ok(dto);
        }
    }
}
