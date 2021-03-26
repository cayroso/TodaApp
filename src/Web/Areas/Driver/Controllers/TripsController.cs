using App.CQRS;
using App.CQRS.Trips.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Data.Common;
using Data.Constants;
using Data.Enums;
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
    public class TripsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public TripsController(IQueryHandlerDispatcher queryHandlerDispatcher)
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
            //var teams = await dbContext.TeamMembers.AsNoTracking().Where(m => m.MemberId == UserId).CountAsync();

            ////  number of members
            //var users = await dbContext.Teams
            //                .AsNoTracking()
            //                .Where(e => e.Members.Any(m => m.MemberId == UserId))
            //                .SelectMany(e => e.Members).Select(e => e.MemberId)
            //                .Distinct()
            //                .CountAsync();

            ////  number of contacts
            //var contacts = await dbContext.Contacts.CountAsync();

            ////  number of documens
            //var documents = await dbContext.Documents.CountAsync();

            ////  number of attachments
            //var attachments = await dbContext.ContactAttachments.CountAsync();

            ////  number of tasks
            //var tasks = await dbContext.UserTasks.CountAsync();

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

        [HttpGet("search")]
        public async Task<IActionResult> GetSearchAsync(EnumTripStatus status, string c, int p, int s, string sf, int so)
        {
            var query = new SearchTripQuery("", TenantId, UserId, status, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchTripQuery, Paged<SearchTripQuery.Trip>>(query);

            return Ok(dto);
        }
    }
}
