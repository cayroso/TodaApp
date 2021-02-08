using Data.App.Models.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Trips
{
    public class TripExcludedDriver
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TripExcludedDriverId { get; set; }

        public string TripId { get; set; }
        public virtual Trip Trip { get; set; }

        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public string Reason { get; set; }
    }
}
