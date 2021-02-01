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
using Web.Areas.Member.Models;
using Web.Controllers;

namespace Web.Areas.Member.Controllers
{
    [Authorize(Policy = ApplicationRoles.MemberRoleName)]
    [ApiController]
    [Route("api/members/[controller]")]
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
            var now = DateTime.UtcNow.Date;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1);

            //  new contacts this month
            var sqlNewContacts = from c in appDbContext.Contacts.AsNoTracking()
                                 where c.AssignedToId == UserId
                                 where c.DateCreated >= startDate && c.DateCreated <= endDate

                                 select new NewContact
                                 {
                                     ContactId = c.ContactId,
                                     StatusText = c.Status.ToString(),
                                     Rating = c.Rating,
                                     FirstName = c.FirstName,
                                     MiddleName = c.MiddleName,
                                     LastName = c.LastName
                                 };

            var dtoNewContacts = await sqlNewContacts.ToListAsync();

            var sqlRecentAttachments = from att in appDbContext.ContactAttachments.AsNoTracking()
                                       where att.Contact.AssignedToId == UserId
                                       where att.DateCreated >= startDate && att.DateCreated <= endDate

                                       select new RecentAttachment
                                       {
                                           ContactAttachmentId = att.ContactAttachmentId,
                                           ContactId = att.ContactId,
                                           AttachmentType = att.AttachmentType,
                                           Title = att.Title,
                                           Content = att.Content,

                                           ContentType = att.FileUpload != null ? att.FileUpload.ContentType : string.Empty,
                                           FileName = att.FileUpload != null ? att.FileUpload.FileName : string.Empty,
                                           Length = att.FileUpload != null ? att.FileUpload.Length : 0,
                                           Url = att.FileUpload != null ? att.FileUpload.Url : string.Empty,
                                       };

            var dtoRecentAttachments = await sqlRecentAttachments.Take(10).ToListAsync();

            var thisWeekStart = now.AddDays(-(int)now.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7);

            var sqlNewTasks = from task in appDbContext.UserTasks.AsNoTracking()
                              where task.Contact.AssignedToId == UserId
                              where task.Status!= Data.Enums.EnumTaskStatus.Done
                                    //&& task.DateCompleted >= now
                                    && task.DateAssigned >= thisWeekStart
                                    && task.DateAssigned <= thisWeekEnd

                              select new Models.Task
                              {
                                  TaskId = task.UserTaskId,

                                  ContactId = task.Contact.ContactId,
                                  FirstName = task.Contact.FirstName,
                                  MiddleName = task.Contact.MiddleName,
                                  LastName = task.Contact.LastName,

                                  TaskStatus = task.Status,
                                  TaskType = task.Type,
                                  Title = task.Title,
                                  Description = task.Description
                              };
            var dtoNewTasks = await sqlNewTasks.ToListAsync();

            var sqlUpcomingTasks = from task in appDbContext.UserTasks.AsNoTracking()
                                   where task.Contact.AssignedToId == UserId
                                   where task.Status != Data.Enums.EnumTaskStatus.Done
                                        //&& task.DateActualCompleted > now
                                        && task.DateCompleted > thisWeekEnd
                                        //&& task.DateCompleted <= thisWeekEnd

                                   select new Models.Task
                                   {
                                       TaskId = task.UserTaskId,

                                       ContactId = task.Contact.ContactId,
                                       FirstName = task.Contact.FirstName,
                                       MiddleName = task.Contact.MiddleName,
                                       LastName = task.Contact.LastName,

                                       TaskStatus = task.Status,
                                       TaskType = task.Type,
                                       Title = task.Title,
                                       Description = task.Description
                                   };
            var dtoUpcomingTasks = await sqlUpcomingTasks.ToListAsync();

            var sqlOverdueTasks = from task in appDbContext.UserTasks.AsNoTracking()
                                  where task.Contact.AssignedToId == UserId
                                  where task.Status != Data.Enums.EnumTaskStatus.Done
                                        //&& task.DateActualCompleted > now
                                        && task.DateCompleted < now

                                  select new Models.Task
                                  {
                                      TaskId = task.UserTaskId,

                                      ContactId = task.Contact.ContactId,
                                      FirstName = task.Contact.FirstName,
                                      MiddleName = task.Contact.MiddleName,
                                      LastName = task.Contact.LastName,

                                      TaskStatus = task.Status,
                                      TaskType = task.Type,
                                      Title = task.Title,
                                      Description = task.Description,
                                  };
            var dtoOverdueTasks = await sqlOverdueTasks.ToListAsync();

            var dto = new DashboardView
            {
                NewContacts = dtoNewContacts,
                RecentAttachments = dtoRecentAttachments,
                NewTasks = dtoNewTasks,
                UpcomingTasks = dtoUpcomingTasks,
                OverdueTasks = dtoOverdueTasks,
            };

            return Ok(dto);
        }
    }
}
