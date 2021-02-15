using Data.App.Models.Trips;
using Data.App.Models.Users;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Drivers
{
    public class Driver
    {
        public string DriverId { get; set; }
        public virtual User User { get; set; }

        public EnumDriverAvailability Availability { get; set; } = EnumDriverAvailability.Available;

        public double OverallRating { get; set; }
        public double TotalRating { get; set; }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();


        public void CalculateRating(double newRating)
        {
            OverallRating = ((OverallRating * TotalRating) + newRating) / (TotalRating + 1);
            TotalRating += newRating;
        }
    }

    public class Vehicle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleId { get; set; }

        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public string PlateNumber { get; set; }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }

}
