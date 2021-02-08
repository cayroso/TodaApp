using App.CQRS;
using Data.App.DbContext;
using Data.App.Models.Trips;
using Data.App.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FeedbacksController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public FeedbacksController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpPost("addOrUpdate")]
        public async Task<IActionResult> AddFeedbackAsync([FromServices] AppDbContext appDbContext, [FromBody] AddFeedback info)
        {
            var trip = await appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == info.TripId);

            trip.ThrowIfNull();

            User user = null;
            List<Trip> completedTrips = new List<Trip>();

            if (info.IsForDriverFeedback)
            {
                trip.DriverComment = info.Comment;
                trip.DriverRating = info.Rate;

                user = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == trip.DriverId);
                completedTrips = await appDbContext.Trips.Where(e => e.DriverId == trip.DriverId && e.Status == Data.Enums.EnumTripStatus.Complete).ToListAsync();
            }
            else
            {
                trip.RiderComment = info.Comment;
                trip.RiderRating = info.Rate;

                user = await appDbContext.Users.FirstOrDefaultAsync(e => e.UserId == trip.RiderId);
                completedTrips = await appDbContext.Trips.Where(e => e.RiderId == trip.RiderId && e.Status == Data.Enums.EnumTripStatus.Complete).ToListAsync();
            }

            user.TotalRating = 0;
            user.OverallRating = 0;

            completedTrips.ForEach(t =>
            {
                user.CalculateRating(t.DriverRating);
            });

            await appDbContext.SaveChangesAsync();

            return Ok();
        }

        public class AddFeedback
        {
            public string TripId { get; set; }
            public bool IsForDriverFeedback { get; set; }
            public string Comment { get; set; }
            public int Rate { get; set; }
        }
    }

}
