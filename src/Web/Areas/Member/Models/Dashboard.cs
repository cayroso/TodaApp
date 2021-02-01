using Common.Extensions;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Member.Models
{
    public class DashboardView
    {
        public List<NewContact> NewContacts { get; set; }
        public List<RecentAttachment> RecentAttachments { get; set; }
        public List<Task> NewTasks { get; set; }
        public List<Task> UpcomingTasks { get; set; }
        public List<Task> OverdueTasks { get; set; }
    }

    public class NewContact
    {
        public string ContactId { get; set; }        
        public string StatusText { get; set; }
        public int Rating { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class RecentAttachment
    {
        public string ContactAttachmentId { get; set; }

        public string ContactId { get; set; }
        public EnumContactAttachmentType AttachmentType { get; set; }
        public string AttachmentTypeText => AttachmentType.ToString();

        public string Url { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class Task
    {
        public string TaskId { get; set; }

        public string ContactId { get; set; }        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public EnumTaskType TaskType { get; set; }
        public string TaskTypeText => TaskType.ToString();

        public EnumTaskStatus TaskStatus { get; set; }
        public string TaskStatusText => TaskStatus.ToString();

        public string Title { get; set; }
        public string Description { get; set; }
    }

}
