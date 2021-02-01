using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Member.Models
{
    public class AddTaskInfo
    {
        [Required]
        public string ContactId { get; set; }

        [Required]
        public EnumTaskType TaskType { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateToComplete { get; set; }

        [Required]
        public List<TaskItemInfo> TaskItems { get; set; }
    }

    public class TaskItemInfo
    {
        [Required]
        public string Title { get; set; }
    }
}
