using Common.Extensions;
using Data.App.Models.FileUploads;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Contacts
{
    public class ContactAttachment
    {
        public string ContactAttachmentId { get; set; }

        public EnumContactAttachmentType AttachmentType { get; set; }

        public string ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public string Title { get; set; }

        public string FileUploadId { get; set; }
        public virtual FileUpload FileUpload { get; set; }

        public string Content { get; set; }

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
    }

    public static class ContactAttachmentExtension
    {
        public static void ThrowIfNull(this ContactAttachment me)
        {
            if (me == null)
                throw new ApplicationException("Not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this ContactAttachment me, string currentToken, string newToken)
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
