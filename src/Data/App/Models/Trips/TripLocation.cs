using Common.Extensions;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Trips
{
    public class TripLocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TripLocationId { get; set; }
        public string TripId { get; set; }
        public virtual Trip Trip { get; set; }

        public EnunTripLocationType TripLocationType { get; set; }

        public double GeoX { get; set; }
        public double GeoY { get; set; }

        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }
    }
}
