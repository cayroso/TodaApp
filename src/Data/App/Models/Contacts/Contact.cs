using Common.Extensions;
using Data.App.Models.Accounts;
using Data.App.Models.Teams;
using Data.App.Models.Users;
using Data.Common;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Contacts
{
    public class Contact
    {
        public string ContactId { get; set; }

        public string AccountId { get; set; }
        public virtual Account Account { get; set; }

        public string CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }

        public string TeamId { get; set; }
        public virtual Team Team { get; set; }

        public string AssignedToId { get; set; }
        public virtual User AssignedTo { get; set; }

        public EnumContactStatus Status { get; set; }
        public EnumContactSalutation Salutation { get; set; }

       
        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string HomePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }

        public int Rating { get; set; }

        DateTime _dateOfInitialContact;
        public DateTime DateOfInitialContact
        {
            get => _dateOfInitialContact;
            set => _dateOfInitialContact = value.Truncate().AsUtc();
        }

        public string ReferralSource { get; set; }

        public string Title { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public decimal AnnualRevenue { get; set; }


        DateTime _dateCreated;
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => _dateCreated = value.Truncate().AsUtc();
        }

        DateTime _dateUpdated;
        public DateTime DateUpdated
        {
            get => _dateUpdated;
            set => _dateUpdated = value.Truncate().AsUtc();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<ContactAttachment> Attachments { get; set; } = new List<ContactAttachment>();
        public virtual ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();

    }

    public static class ContactExtension
    {
        public static void ThrowIfNull(this Contact me)
        {
            if (me == null)
                throw new ApplicationException("Not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Contact me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("Already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }
}
