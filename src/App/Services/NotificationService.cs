using Cayent.Core.Common;
using Data.App.DbContext;

using Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cayent.Core.Common.Extensions;

namespace App.Services
{
    //public class NotificationService
    //{
    //    private readonly AppDbContext _appDbContext;

    //    public NotificationService(AppDbContext appDbContext)
    //    {
    //        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    //    }

    //    public async Task AddNotification(string referenceId, string iconClass, string subject, string content,
    //        EnumNotificationType notificationType, string[] userIds, string[] roleNames)
    //    {
    //        if ((userIds == null || !userIds.Any()) && (roleNames == null || !roleNames.Any()))
    //        {
    //            await Task.CompletedTask;
    //        }

    //        var notificationId = Guid.NewGuid().ToString();
    //        var notification = new Notification
    //        {
    //            NotificationId = notificationId,
    //            IconClass = iconClass,
    //            Subject = subject,
    //            Content = content,
    //            ReferenceId = referenceId,
    //            DateSent = DateTime.UtcNow,
    //            NotificationType = notificationType,
    //        };

    //        var existingNotification = await _appDbContext.Notifications.Include(e=> e.Receivers).FirstOrDefaultAsync(e => e.ReferenceId == referenceId);

    //        if (existingNotification != null)
    //        {
    //            notificationId = existingNotification.NotificationId;
    //            notification = existingNotification;

    //            notification.NotificationId = notificationId;
    //            notification.IconClass = iconClass;
    //            notification.Subject = subject;
    //            notification.Content = content;
    //            notification.ReferenceId = referenceId;
    //            notification.DateSent = DateTime.UtcNow;
    //            notification.NotificationType = notificationType;                
    //        }

    //        var combinedUserIds = new List<string>();

    //        if (userIds != null && userIds.Any())
    //        {
    //            foreach (var userId in userIds)
    //            {
    //                var alreadyReceiver = notification.Receivers.Any(e => e.ReceiverId == userId);
    //                if (!alreadyReceiver)
    //                {
    //                    notification.Receivers.Add(new NotificationReceiver
    //                    {
    //                        NotificationId = notification.NotificationId,
    //                        ReceiverId = userId,
    //                        DateRead = DateTime.MaxValue,
    //                        DateReceived = notification.DateSent
    //                    });

    //                    combinedUserIds.Add(userId);
    //                }
    //            }
    //        }

    //        if (roleNames != null && roleNames.Any())
    //        {
    //            foreach (var roleName in roleNames)
    //            {
    //                //var urs = await _dbContext
    //                //    .UserRoles
    //                //    .Include(p => p.Role)
    //                //    .Where(p => p.Role.Name == roleName)
    //                //    .ToListAsync();

    //                //urs.ForEach(p =>
    //                //{
    //                //    notification.Receivers.Add(new NotificationReceiver
    //                //    {
    //                //        NotificationId = notification.NotificationId,
    //                //        ReceiverId = p.UserId,
    //                //        DateRead = DateTimeOffset.MaxValue,
    //                //        DateReceived = notification.DateSent
    //                //    });

    //                //    combinedUserIds.Add(p.UserId);
    //                //});
    //            }
    //        }

    //        if (existingNotification == null)
    //        {
    //            await _appDbContext.AddAsync(notification);
    //        }

    //        await _appDbContext.SaveChangesAsync();

    //        //if (combinedUserIds.Any())
    //        //    await _hubContext.Clients.Users(combinedUserIds).Received(notification);

    //        await Task.CompletedTask;

    //    }

    //    public async Task<Paged<SearchNotificationInfo>> GetNotificationsAsync(string userId, bool onlyUnread, string criteria, int pageIndex = 1, int pageSize = 10)
    //    {
    //        var query = from nr in _appDbContext.NotificationReceivers
    //                    join n in _appDbContext.Notifications on nr.NotificationId equals n.NotificationId
    //                    where nr.ReceiverId == userId && (onlyUnread == false || DateTime.UtcNow <= nr.DateRead)//nr.IsRead == false)
    //                    where criteria == null || EF.Functions.Like(n.Subject, $"%{criteria}%") || EF.Functions.Like(n.Content, $"%{criteria}%")
    //                    select new SearchNotificationInfo
    //                    {
    //                        NotificationId = n.NotificationId,
    //                        NotificationType = (int)n.NotificationType,
    //                        Content = n.Content,
    //                        DateSent = n.DateSent,
    //                        IconClass = n.IconClass,
    //                        ReferenceId = n.ReferenceId,
    //                        Subject = n.Subject,
    //                        DateRead = nr.DateRead,
    //                        DateReceived = nr.DateReceived
    //                    };

    //        var pagedItems = await query
    //            .AsNoTracking()
    //            .ToPagedItemsAsync(pageIndex, pageSize);

    //        pagedItems.Items.ToList().ForEach(item =>
    //        {
    //            item.DateRead = DateTime.SpecifyKind(item.DateRead, DateTimeKind.Utc);
    //            item.DateReceived = DateTime.SpecifyKind(item.DateReceived, DateTimeKind.Utc);
    //            item.DateSent = DateTime.SpecifyKind(item.DateSent, DateTimeKind.Utc);
    //        });
    //        //var paginated = new Paginated<SearchNotificationInfo>(pagedItems, pagedItems.TotalCount, pagedItems.PageIndex, pagedItems.PageSize);

    //        return await Task.FromResult(pagedItems);
    //    }

    //    public async Task<SearchNotificationInfo> GetNotificationAsync(string userId, string notificationId)
    //    {
    //        var query = from nr in _appDbContext.NotificationReceivers
    //                    join n in _appDbContext.Notifications on nr.NotificationId equals n.NotificationId
    //                    where n.NotificationId == notificationId && nr.ReceiverId == userId
    //                    select new SearchNotificationInfo
    //                    {
    //                        NotificationId = n.NotificationId,
    //                        Content = n.Content,
    //                        DateRead = nr.DateRead,
    //                        DateReceived = nr.DateReceived,
    //                        DateSent = n.DateSent,
    //                        IconClass = n.IconClass,
    //                        NotificationType = (int)n.NotificationType,
    //                        ReferenceId = n.ReferenceId,
    //                        Subject = n.Subject
    //                    };

    //        var data = await query
    //            .AsNoTracking()
    //            .FirstOrDefaultAsync();

    //        if (data != null)
    //        {
    //            data.DateRead = DateTime.SpecifyKind(data.DateRead, DateTimeKind.Utc);
    //            data.DateReceived = DateTime.SpecifyKind(data.DateReceived, DateTimeKind.Utc);
    //            data.DateSent = DateTime.SpecifyKind(data.DateSent, DateTimeKind.Utc);

    //            return await Task.FromResult(data);
    //        }

    //        return await Task.FromResult<SearchNotificationInfo>(null);
    //    }

    //    public async Task MarkNotificationAsRead(string userId, string notificationId)
    //    {
    //        var data = await _appDbContext
    //            .NotificationReceivers
    //            .Include(p => p.Notification)
    //            .Where(p => p.NotificationId == notificationId)
    //            .Where(p => p.ReceiverId == userId)

    //            .FirstOrDefaultAsync();

    //        if (data != null)
    //        {
    //            data.DateRead = DateTime.UtcNow;

    //            await _appDbContext.SaveChangesAsync();
    //        }

    //        await Task.CompletedTask;
    //    }

    //    public async Task DeleteNotification(string userId, string notificationId)
    //    {
    //        var data = await _appDbContext
    //            .Notifications
    //            .Include(p => p.Receivers)
    //            .Where(p => p.NotificationId == notificationId)
    //            .Where(p => p.Receivers.Any(q => q.ReceiverId == userId))
    //            .FirstOrDefaultAsync();

    //        if (data != null)
    //        {
    //            var toRemove = data.Receivers.First(p => p.ReceiverId == userId);

    //            if (toRemove != null)
    //            {
    //                data.Receivers.Remove(toRemove);
    //                _appDbContext.Remove(toRemove);
    //            }

    //            if (!data.Receivers.Any())
    //            {
    //                _appDbContext.Remove(data);
    //            }

    //            await _appDbContext.SaveChangesAsync();
    //        }

    //        await Task.CompletedTask;
    //    }

    //    public async Task DeleteNotificationByReferenceId(string referenceId)
    //    {
    //        var data = await _appDbContext
    //            .Notifications                
    //            .Where(p => p.ReferenceId == referenceId)                
    //            .FirstOrDefaultAsync();

    //        if (data != null)
    //        {
    //            _appDbContext.Remove(data);

    //            await _appDbContext.SaveChangesAsync();
    //        }

    //        await Task.CompletedTask;
    //    }

    //    public async Task DeleteNotificationByReferenceId(string referenceId, string userId)
    //    {
    //        var data = await _appDbContext
    //            .Notifications
    //            .Include(p => p.Receivers)
    //            .Where(p => p.ReferenceId == referenceId)
    //            .Where(p => p.Receivers.Any(q => q.ReceiverId == userId))
    //            .FirstOrDefaultAsync();

    //        if (data != null)
    //        {
    //            var toRemove = data.Receivers.First(p => p.ReceiverId == userId);

    //            if (toRemove != null)
    //            {
    //                data.Receivers.Remove(toRemove);
    //                _appDbContext.Remove(toRemove);
    //            }

    //            if (!data.Receivers.Any())
    //            {
    //                _appDbContext.Remove(data);
    //            }

    //            await _appDbContext.SaveChangesAsync();
    //        }

    //        await Task.CompletedTask;
    //    }

    //    public class SearchNotificationInfo
    //    {
    //        public string NotificationId { get; set; }
    //        public int NotificationType { get; set; }
    //        public string IconClass { get; set; }
    //        public string Subject { get; set; }
    //        public string Content { get; set; }
    //        public string ReferenceId { get; set; }
    //        public DateTime DateSent { get; set; }
    //        public DateTime DateReceived { get; set; }
    //        public DateTime DateRead { get; set; }

    //        public bool IsRead => DateRead < DateTime.UtcNow;
    //    }
    //}
}
