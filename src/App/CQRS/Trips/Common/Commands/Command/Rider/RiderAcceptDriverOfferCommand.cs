using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cayent.Core.CQRS.Commands;

namespace App.CQRS.Trips.Common.Commands.Command.Rider
{
    public sealed class RiderAcceptDriverOfferCommand : AbstractCommand
    {
        public string TripId { get; }
        public string RiderId { get; }
        public string Token { get; }

        public RiderAcceptDriverOfferCommand(string correlationId, string tenantId, string userId,
            string tripId, string riderId, string token)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
            RiderId = riderId;
            Token = token;
        }
    }
}
