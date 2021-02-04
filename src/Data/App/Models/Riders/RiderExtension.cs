using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Riders
{    
    public static class RiderExtension
    {
        public static void ThrowIfNull(this Rider me)
        {
            if (me == null)
                throw new ApplicationException("Rider not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Rider me, string currentToken, string newToken)
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
