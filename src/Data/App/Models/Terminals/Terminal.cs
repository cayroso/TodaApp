using Common.Extensions;
using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Terminals
{
    public enum EnumRideStatus
    {
        Unknown = 0,
        /// <summary>
        /// Ride is requested by not yet processed by the system
        /// </summary>
        Requested = 1,
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
        /// Rider accepted the fare offered by driver, system will put ride to InProgress
        /// </summary>
        RiderAccepted,
        /// <summary>
        /// Rider rejected fare offered, system will put ride to Requested
        /// </summary>
        RiderRejected,
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

    public class Ride
    {
        public string RideId { get; set; }

        public EnumRideStatus Status { get; set; }
        public string CancelReason { get; set; }

        public string RiderId { get; set; }
        public virtual User Rider { get; set; }

        public string DriverId { get; set; }
        public virtual User Driver { get; set; }

        public string StartAddress { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }

        public string EndAddress { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }

        public decimal Fare { get; set; }

        DateTime _dateCreated;
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        DateTime _dateUpdated;
        public DateTime DateUpdated
        {
            get => _dateUpdated.AsUtc();
            set => _dateUpdated = value.Truncate();
        }

        DateTime _dateDeleted;
        public DateTime DateDeleted
        {
            get => _dateDeleted.AsUtc();
            set => _dateDeleted = value.Truncate();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<RideTimeline> Timelines { get; set; } = new List<RideTimeline>();
    }

    public class RideTimeline
    {
        public string RideTimelineId { get; set; }

        public string RideId { get; set; }
        public virtual Ride Ride { get; set; }

        public EnumRideStatus Status { get; set; }

        DateTime _dateTimeline;
        public DateTime DateTimeline
        {
            get => _dateTimeline.AsUtc();
            set => _dateTimeline = value.Truncate();
        }
    }    
}
