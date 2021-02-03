using App.CQRS;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.App.Models.Teams;
using Data.Common;
using Data.Identity.DbContext;
using Data.Providers;
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
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RidesController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;

        public RidesController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        public class AddRideInfo
        {
            public string StartAddress { get; set; }
            public string EndAddress { get; set; }
        }
    }
}
