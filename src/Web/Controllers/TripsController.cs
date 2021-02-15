﻿using App.CQRS;
using App.CQRS.Trips.Common.Commands.Command;
using App.CQRS.Trips.Common.Commands.Command.Driver;
using App.CQRS.Trips.Common.Commands.Command.Rider;
using App.CQRS.Trips.Common.Queries.Query;
using App.Hubs;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.App.Models.Trips;
using Data.Common;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.BackgroundServices;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TripsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        readonly ICommandHandlerDispatcher _commandHandlerDispatcher;
        readonly AppDbContext _appDbContext;
        readonly IHubContext<TripHub, ITripClient> _tripHubContext;

        public TripsController(
            IQueryHandlerDispatcher queryHandlerDispatcher,
            ICommandHandlerDispatcher commandHandlerDispatcher,
            AppDbContext appDbContext,
            IHubContext<TripHub, ITripClient> tripHubContext)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _tripHubContext = tripHubContext ?? throw new ArgumentNullException(nameof(tripHubContext));
        }

        #region Driver

        [HttpPut("driver/accept-rider-request")]
        public async Task<IActionResult> PutDriverAcceptRiderTripRequestAsync([FromBody] DriverAcceptRiderTripRequestInfo info)
        {
            var cmd = new DriverAcceptRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("driver/reject-rider-request")]
        public async Task<IActionResult> PutDriverRejectRiderTripRequestAsync(
            //[FromServices] AppDbContext appDbContext,
            [FromServices] JobQueue<Trip> job,
            [FromBody] DriverRejectRiderTripRequestInfo info)
        {
            var cmd = new DriverRejectRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            var trip = await _appDbContext.Trips.AsNoTracking().FirstOrDefaultAsync(e => e.TripId == cmd.TripId);
            job.Enqueue(trip);

            return Ok();
        }

        [HttpPut("driver/offer-fare-to-rider-request")]
        public async Task<IActionResult> PutDriverOfferFareToRiderTripRequestAsync([FromBody] DriverOfferFareToRiderTripRequestInfo info)
        {
            var cmd = new DriverOfferFareToRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Fare, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("{id}/driver-complete/{token}")]
        public async Task<IActionResult> PutDriverTripToCompleteAsync(string id, string token)
        {
            var cmd = new SetTripToCompleteCommand("", TenantId, UserId, id, token, false);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("{id}/driver-inprogress/{token}")]
        public async Task<IActionResult> PutDriverTripToInCompleteAsync(string id, string token)
        {
            var cmd = new SetTripToInProgressCommand("", TenantId, UserId, id, token, false);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }


        #endregion

        #region Rider

        [HttpPost("rider/add")]
        public async Task<IActionResult> PostRiderAddTripAsync([FromBody] RiderCreateTripInfo info)
        {
            var cmd = new RiderCreateTripCommand("", TenantId, UserId, GuidStr(), UserId,
                info.StartAddress, info.StartAddressDescription, info.StartX, info.StartY,
                info.EndAddress, info.EndAddressDescription, info.EndX, info.EndY);

            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok(cmd.TripId);
        }

        [HttpPut("rider/request")]
        public async Task<IActionResult> PutRiderRequestTripAsync(
            [FromServices] AppDbContext appDbContext,
            [FromServices] JobQueue<Trip> job,
            [FromBody] RiderRequestTripInfo info)
        {
            var cmd = new RiderRequestTripCommand("", TenantId, UserId, info.TripId, UserId, info.Token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            var trip = await appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == cmd.TripId);
            job.Enqueue(trip);

            //await _tripHubContext.Clients.All.TripRequested();

            return Ok();
        }

        [HttpPut("rider/cancel")]
        public async Task<IActionResult> PutRiderCancelTripAsync([FromBody] RiderCancelTripInfo info)
        {
            var cmd = new RiderCancelTripCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("rider/accept-driver-offer")]
        public async Task<IActionResult> PutRiderAcceptDriverOfferAsync([FromBody] RiderAcceptDriverOfferInfo info)
        {
            var cmd = new RiderAcceptDriverOfferCommand("", TenantId, UserId, info.TripId, UserId, info.Token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("rider/reject-driver-offer")]
        public async Task<IActionResult> PutRiderRejectDriverOfferAsync(
            //[FromServices] AppDbContext appDbContext,
            [FromServices] JobQueue<Trip> job,
            [FromBody] RiderRejectDriverOfferInfo info)
        {
            var cmd = new RiderRejectDriverOfferCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            var trip = await _appDbContext.Trips.AsNoTracking().FirstOrDefaultAsync(e => e.TripId == cmd.TripId);
            job.Enqueue(trip);

            return Ok();
        }


        [HttpPut("{id}/rider-complete/{token}")]
        public async Task<IActionResult> PutRiderTripToCompleteAsync(string id, string token)
        {
            var cmd = new SetTripToCompleteCommand("", TenantId, UserId, id, token, true);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }

        [HttpPut("{id}/rider-inprogress/{token}")]
        public async Task<IActionResult> PutRiderTripToInCompleteAsync(string id, string token)
        {
            var cmd = new SetTripToInProgressCommand("", TenantId, UserId, id, token, true);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }


        #endregion

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripAsync(string id)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, id);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);

            return Ok(dto);
        }


        #region Feedback

        [HttpPut("rider-feedback")]
        public async Task<IActionResult> PutRiderReview([FromBody] AddFeedbackInfo info)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var data = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == info.TripId);

            data.ThrowIfNull();

            data.DriverRating = info.Rating;
            data.DriverComment = info.Comment;

            var user = await _appDbContext.Drivers.FirstOrDefaultAsync(e => e.DriverId == data.DriverId);

            //get other driver's trips
            var trips = await _appDbContext.Trips.AsNoTracking().Where(e => e.TripId != info.TripId && e.DriverId == data.DriverId).ToListAsync();

            user.OverallRating = 0;
            user.TotalRating = 0;

            user.CalculateRating(info.Rating);
            trips.ForEach(t =>
            {
                user.CalculateRating(t.DriverRating);
            });

            await _appDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("driver-feedback")]
        public async Task<IActionResult> PutDriverReview([FromBody] AddFeedbackInfo info)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var data = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == info.TripId);

            data.ThrowIfNull();

            data.RiderRating = info.Rating;
            data.RiderComment = info.Comment;

            var user = await _appDbContext.Riders.FirstOrDefaultAsync(e => e.RiderId == data.RiderId);

            //get all rider's trips
            var trips = await _appDbContext.Trips.AsNoTracking().Where(e => e.TripId != info.TripId && e.DriverId == data.RiderId).ToListAsync();

            user.OverallRating = 0;
            user.TotalRating = 0;

            user.CalculateRating(info.Rating);
            trips.ForEach(t =>
            {
                user.CalculateRating(t.RiderRating);
            });

            await _appDbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
