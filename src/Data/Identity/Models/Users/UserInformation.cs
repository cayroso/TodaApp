using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Identity.Models.Users
{
    public class UserInformation
    {
        public string UserId { get; set; }
        public virtual IdentityWebUser User { get; set; }
        public string ImageId { get; set; }
        //public virtual Image Image { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FirstLastName => $"{FirstName} {LastName}";

        public string Theme { get; set; }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();
    }

    public static class UserInformationExtension
    {
        public static void ThrowIfNull(this UserInformation me)
        {
            if (me == null)
                throw new ApplicationException("Not found.");
        }

        public static void ThrowIfNullOrAlreadyUpdated(this UserInformation me, string currentToken, string newToken)
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
