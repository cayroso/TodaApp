using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Command
{
    public sealed class RiderRejectedDriverOfferCommand : AbstractCommand
    {
        public string TripId { get; }
        public string RiderId { get; }
        public string Token { get; }
        public string Notes { get;   }

        public RiderRejectedDriverOfferCommand(string correlationId, string tenantId, string userId,
            string tripId, string riderId, string token, string notes)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
            RiderId = riderId;
            Token = token;
            Notes = notes;
        }
    }
}
