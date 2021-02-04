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
        public async Task<IActionResult> GetDashboard([FromServices] AppDbContext dbContext, int year, int month)
        {
            //var now = DateTime.UtcNow;
            //var startDate = new DateTime(now.Year, now.Month, 1);
            //var endDate = startDate.AddMonths(1).AddDays(-1);

            ////  number of teams
            //var teams = await dbContext.Teams.AsNoTracking().CountAsync();

            ////  number of users
            //var users = await dbContext.Users.AsNoTracking().CountAsync();

            ////  number of contacts
            //var contacts = await dbContext.Contacts.AsNoTracking().CountAsync();

            ////  number of documens
            //var documents = await dbContext.Documents.AsNoTracking().CountAsync();

            ////  number of attachments
            //var attachments = await dbContext.ContactAttachments.AsNoTracking().CountAsync();

            ////  number of tasks
            //var tasks = await dbContext.UserTasks.AsNoTracking().CountAsync();

            //var dto = new
            //{
            //    teams,
            //    users,
            //    contacts,
            //    documents,
            //    attachments,
            //    tasks
            //};

            //return Ok(dto);
            return Ok();
        }

        public class DashboardView
        {

        }
    }
}
