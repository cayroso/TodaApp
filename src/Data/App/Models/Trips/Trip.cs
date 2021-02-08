using Common.Extensions;
using Data.App.Models.Drivers;
using Data.App.Models.Riders;
using Data.App.Models.Users;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Trips
{
    public class Trip
    {
        public string TripId { get; set; }

        public EnumTripStatus Status { get; set; }
        public string CancelReason { get; set; }

        public string RiderId { get; set; }
        public virtual Rider Rider { get; set; }

        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public string VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public string StartAddress { get; set; }
        public string StartAddressDescription { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }

        public string EndAddress { get; set; }
        public string EndAddressDescription { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }

        public decimal Fare { get; set; }

        public int RiderRating { get; set; }
        public string RiderComment { get; set; }

        public int DriverRating { get; set; }
        public string DriverComment { get; set; }


        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<TripExcludedDriver> ExcludedDrivers { get; set; } = new List<TripExcludedDriver>();
        public virtual ICollection<TripLocation> Locations { get; set; } = new List<TripLocation>();
        public virtual ICollection<TripTimeline> Timelines { get; set; } = new List<TripTimeline>();
    }
}
