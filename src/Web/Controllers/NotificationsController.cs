using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class NotificationsController : BaseController
    {
        [HttpGet("")]
        public async Task<IActionResult> SearchNotifications([FromServices] NotificationService notificationService, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var paginated = await notificationService.GetNotificationsAsync(UserId, false, criteria, pageIndex, pageSize);

            return Ok(paginated);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> SearchUnreadNotifications([FromServices] NotificationService notificationService, string criteria, int pageIndex = 1, int pageSize = 10)
        {
            var paginated = await notificationService.GetNotificationsAsync(UserId, true, criteria, pageIndex, pageSize);

            return Ok(paginated);
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetNotification([FromServices] NotificationService notificationService, string notificationId)
        {
            var model = await notificationService.GetNotificationAsync(UserId, notificationId);

            return Ok(model);
        }

        [HttpPost("{notificationId}/markAsRead")]
        public async Task<IActionResult> MarkAsRead([FromServices] NotificationService notificationService, string notificationId)
        {
            await notificationService.MarkNotificationAsRead(UserId, notificationId);

            return Ok();
        }

        [HttpPost("{notificationId}/delete")]
        public async Task<IActionResult> DeleteNotification([FromServices] NotificationService notificationService, string notificationId)
        {
            await notificationService.DeleteNotification(UserId, notificationId);

            return Ok();
        }
    }
}
