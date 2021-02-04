using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Command
{
    public sealed class RiderCreatedTripCommand : AbstractCommand
    {
        public string TripId { get; }
        public string RiderId { get; }
        public string StartAddress { get; }
        public string StartAddressDescription { get; }
        public double StartX { get; }
        public double StartY { get; }

        public string EndAddress { get; }
        public string EndAddressDescription { get; }
        public double EndX { get; }
        public double EndY { get; }

        public RiderCreatedTripCommand(string correlationId, string tenantId, string userId, string tripId, string riderId,
            string startAddress, string startAddressDescription, double startX, double startY,
            string endAddress, string endAddressDescription, double endX, double endY)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
            RiderId = riderId;

            StartAddress = startAddress;
            StartAddressDescription = startAddressDescription;
            StartX = startX;
            StartY = startY;

            EndAddress = endAddress;
            EndAddressDescription = endAddressDescription;
            EndX = endX;
            EndY = endY;
        }
    }
}
