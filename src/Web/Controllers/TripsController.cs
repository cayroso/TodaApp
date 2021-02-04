using App.CQRS;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.Common;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public TripsController(IQueryHandlerDispatcher queryHandlerDispatcher, ICommandHandlerDispatcher commandHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
        }

        public async Task<IActionResult> Post([FromServices]AppDbContext dbContext, [FromBody] BookTripInfo info)
        {

            return Ok();
        }

        public class BookTripInfo
        {
            public string StartAddress { get; set; }
            public string StartAddressDescription { get; set; }
            public double StartX { get; set; }
            public double StartY { get; set; }

            public string EndAddress { get; set; }
            public string EndAddressDescription { get; set; }
            public double EndX { get; set; }
            public double EndY { get; set; }
        }
    }
}
