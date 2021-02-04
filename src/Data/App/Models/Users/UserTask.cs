using Common.Extensions;
using Data.Enums;
using Data.Identity.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Users
{

    public class UserTask
    {
        public string UserTaskId { get; set; }

        public EnumTaskType Type { get; set; } = EnumTaskType.Unknown;

        public EnumTaskStatus Status { get; set; } = EnumTaskStatus.Unknown;

        public string RoleId { get; set; }
        public virtual Role Role { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }


        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate().AsUtc();
        }

        DateTime _dateAssigned = DateTime.MaxValue;
        public DateTime DateAssigned
        {
            get => _dateAssigned.AsUtc();
            set => _dateAssigned = value.Truncate().AsUtc();
        }

        DateTime _dateCompleted = DateTime.MaxValue;
        public DateTime DateCompleted
        {
            get => _dateCompleted.AsUtc();
            set => _dateCompleted = value.Truncate().AsUtc();
        }

        DateTime _dateActualCompleted = DateTime.MaxValue;
        public DateTime DateActualCompleted
        {
            get => _dateActualCompleted.AsUtc();
            set => _dateActualCompleted = value.Truncate().AsUtc();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<UserTaskItem> UserTaskItems { get; set; } = new List<UserTaskItem>();
    }

    public class UserTaskItem
    {
        public string UserTaskItemId { get; set; }

        public string UserTaskId { get; set; }
        public virtual UserTask UserTask { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; } = false;

        DateTime _dateCompleted = DateTime.MaxValue;
        public DateTime DateCompleted
        {
            get => _dateCompleted.AsUtc();
            set => _dateCompleted = value.Truncate().AsUtc();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();
    }

    public static class UserTaskExtension
    {
        public static void ThrowIfNull(this UserTask me)
        {
            if (me == null)
                throw new ApplicationException("User Task not found.");
        }

        public static void ThrowIfNullOrAlreadyUpdated(this UserTask me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("User Task already updated by another user.");

            me.ConcurrencyToken = newToken;
        }


        public static void ThrowIfNull(this UserTaskItem me)
        {
            if (me == null)
                throw new ApplicationException("User Task Item not found.");
        }

        public static void ThrowIfNullOrAlreadyUpdated(this UserTaskItem me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("User Task already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }
}
