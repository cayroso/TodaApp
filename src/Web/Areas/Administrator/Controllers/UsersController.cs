using Cayent.Core.CQRS.Queries;
using Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Web.Controllers;

namespace Web.Areas.Administrator.Controllers
{
    [Authorize(Policy = ApplicationRoles.AdministratorRoleName)]
    [ApiController]
    [Route("api/administrators/[controller]")]
    [Produces("application/json")]
    public class UsersController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public UsersController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers()
        {
            return Ok();
        }
    }
}
