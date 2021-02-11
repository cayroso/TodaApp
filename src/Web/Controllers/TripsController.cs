using App.CQRS;
using App.CQRS.Trips.Common.Commands.Command;
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
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);

            var cmd = new DriverAcceptRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Rider.RiderId }).DriverAccepted(new ViewModel.Trips.DriverAccepted.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

            return Ok();
        }

        [HttpPut("driver/reject-rider-request")]
        public async Task<IActionResult> PutDriverRejectRiderTripRequestAsync(
            //[FromServices] AppDbContext appDbContext,
            [FromServices] JobQueue<Trip> job,
            [FromBody] DriverRejectRiderTripRequestInfo info)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);

            var cmd = new DriverRejectRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            var trip = await _appDbContext.Trips.AsNoTracking().FirstOrDefaultAsync(e => e.TripId == cmd.TripId);
            job.Enqueue(trip);

            await _tripHubContext.Clients.Users(new[] { dto.Rider.RiderId }).DriverRejected(new ViewModel.Trips.DriverRejected.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

            return Ok();
        }

        [HttpPut("driver/offer-fare-to-rider-request")]
        public async Task<IActionResult> PutDriverOfferFareToRiderTripRequestAsync([FromBody] DriverOfferFareToRiderTripRequestInfo info)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new DriverOfferFareToRiderTripRequestCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Fare, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Rider.RiderId }).DriverFareOffered(new ViewModel.Trips.DriverFareOffered.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
                Fare = info.Fare
            });

            return Ok();
        }

        [HttpPut("{id}/driver-complete/{token}")]
        public async Task<IActionResult> PutDriverTripToCompleteAsync(string id, string token)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, id);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new SetTripToCompleteCommand("", TenantId, UserId, id, token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Rider.RiderId }).DriverTripCompleted(new ViewModel.Trips.DriverTripCompleted.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

            return Ok();
        }

        [HttpPut("{id}/driver-inprogress/{token}")]
        public async Task<IActionResult> PutDriverTripToInCompleteAsync(string id, string token)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, id);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new SetTripToInProgressCommand("", TenantId, UserId, id, token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Rider.RiderId }).DriverTripInProgress(new ViewModel.Trips.DriverTripInProgress.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

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
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);

            var cmd = new RiderCancelTripCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Driver.DriverId }).RiderTripCancelled(new ViewModel.Trips.RiderTripCancelled.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver == null ? null : dto.Driver.DriverId,
                DriverName = dto.Driver == null ? null : $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
                Reason = info.Notes
            });

            return Ok();
        }

        [HttpPut("rider/accept-driver-offer")]
        public async Task<IActionResult> PutRiderAcceptDriverOfferAsync([FromBody] RiderAcceptDriverOfferInfo info)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new RiderAcceptDriverOfferCommand("", TenantId, UserId, info.TripId, UserId, info.Token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Driver.DriverId }).RiderOfferedFareAccepted(new ViewModel.Trips.RiderOfferedFareAccepted.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",

                Fare = dto.Fare
            });

            return Ok();
        }

        [HttpPut("rider/reject-driver-offer")]
        public async Task<IActionResult> PutRiderRejectDriverOfferAsync(
            //[FromServices] AppDbContext appDbContext,
            [FromServices] JobQueue<Trip> job,
            [FromBody] RiderRejectDriverOfferInfo info)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, info.TripId);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);

            var cmd = new RiderRejectDriverOfferCommand("", TenantId, UserId, info.TripId, UserId, info.Token, info.Notes);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            var trip = await _appDbContext.Trips.AsNoTracking().FirstOrDefaultAsync(e => e.TripId == cmd.TripId);
            job.Enqueue(trip);

            await _tripHubContext.Clients.Users(new[] { dto.Driver.DriverId }).RiderOfferedFareRejected(new ViewModel.Trips.RiderOfferedFareRejected.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
                Fare = dto.Fare
            });

            return Ok();
        }


        [HttpPut("{id}/rider-complete/{token}")]
        public async Task<IActionResult> PutRiderTripToCompleteAsync(string id, string token)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, id);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new SetTripToCompleteCommand("", TenantId, UserId, id, token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Driver.DriverId }).RiderTripCompleted(new ViewModel.Trips.RiderTripCompleted.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

            return Ok();
        }

        [HttpPut("{id}/rider-inprogress/{token}")]
        public async Task<IActionResult> PutRiderTripToInCompleteAsync(string id, string token)
        {
            var query = new GetTripByIdQuery("", TenantId, UserId, id);
            var dto = await _queryHandlerDispatcher.HandleAsync<GetTripByIdQuery, GetTripByIdQuery.Trip>(query);
            
            var cmd = new SetTripToInProgressCommand("", TenantId, UserId, id, token);
            await _commandHandlerDispatcher.HandleAsync(cmd);

            await _tripHubContext.Clients.Users(new[] { dto.Driver.DriverId }).RiderTripInProgress(new ViewModel.Trips.RiderTripInProgress.Response
            {
                TripId = dto.TripId,
                DriverId = dto.Driver.DriverId,
                DriverName = $"{dto.Driver.FirstName} {dto.Driver.MiddleName} {dto.Driver.LastName}",

                RiderId = dto.Rider.RiderId,
                RiderName = $"{dto.Rider.FirstName} {dto.Rider.MiddleName} {dto.Rider.LastName}",
            });

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

        #endregion
    }
}
