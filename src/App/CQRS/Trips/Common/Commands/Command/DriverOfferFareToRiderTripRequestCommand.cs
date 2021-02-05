using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Command
{
    public sealed class DriverOfferFareToRiderTripRequestCommand : AbstractCommand
    {
        public string TripId { get; }
        public string RiderId { get; }
        public string Token { get; }
        public decimal Fare { get; }
        public string Notes { get; }

        public DriverOfferFareToRiderTripRequestCommand(string correlationId, string tenantId, string userId,
            string tripId, string riderId, string token, decimal fare, string notes)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
            RiderId = riderId;
            Token = token;
            Fare = fare;
            Notes = notes;
        }
    }
}
