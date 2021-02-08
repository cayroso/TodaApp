using Common.Extensions;
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
    public class TripTimeline
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TripTimelineId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string TripId { get; set; }
        public virtual Trip Trip { get; set; }

        public EnumTripStatus Status { get; set; }

        public string Notes { get; set; }

        DateTime _dateTimeline = DateTime.UtcNow.Truncate();
        public DateTime DateTimeline
        {
            get => _dateTimeline.AsUtc();
            set => _dateTimeline = value.Truncate();
        }
    }
}
