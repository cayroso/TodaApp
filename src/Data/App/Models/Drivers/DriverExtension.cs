using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Drivers
{
    public static class DriverExtension
    {
        public static void ThrowIfNull(this Driver me)
        {
            if (me == null)
                throw new ApplicationException("Driver not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Driver me, string currentToken, string newToken)
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
