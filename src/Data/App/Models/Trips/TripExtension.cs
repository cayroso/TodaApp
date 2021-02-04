using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Trips
{
    public static class TripExtension
    {
        public static void ThrowIfNull(this Trip me)
        {
            if (me == null)
                throw new ApplicationException("Trip not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Trip me, string currentToken, string newToken)
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
