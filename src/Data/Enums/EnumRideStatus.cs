using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enums
{
    public enum EnumRideStatus
    {
        Unknown = 0,

        Pending,
        /// <summary>
        /// Ride is requested by not yet processed by the system
        /// </summary>
        Requested,
        /// <summary>
        /// System assigned ride to driver, 5min for driver to accept/reject
        /// </summary>
        DriverAssigned,
        /// <summary>
        /// Driver accepted, system will send fare to rider
        /// </summary>
        DriverAccepted,
        /// <summary>
        /// Driver rejected, system will set this to Requested
        /// </summary>
        DriverRejected,
        /// <summary>
        /// Driver has accepted the trip and now offering fare to rider
        /// </summary>
        DriverOfferedFare,
        /// <summary>
        /// Rider accepted the fare offered by driver, system will put ride to InProgress
        /// </summary>
        RiderOfferedFareAccepted,
        /// <summary>
        /// Rider rejected fare offered, system will put ride to Requested
        /// </summary>
        RiderOfferedFareRejected,
        /// <summary>
        /// Ride is currently in travel
        /// </summary>
        InProgress,
        /// <summary>
        /// Ride has reached destination, either manually set by rider or the system detects that the ride has expired, e.g. travel time calculated is 10mins
        /// </summary>
        Complete,
        /// <summary>
        /// When the rider cancelled the ride
        /// </summary>
        Cancelled
    }
}
