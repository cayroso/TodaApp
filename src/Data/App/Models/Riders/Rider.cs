using Data.App.Models.Trips;
using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Riders
{
    public class Rider
    {
        public string RiderId { get; set; }
        public virtual User User { get; set; }
        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<RiderBookmark> Bookmarks { get; set; } = new List<RiderBookmark>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();        
    }

    public class RiderBookmark
    {
        public string RiderBookmarkId { get; set; }

        public string RiderId { get; set; }
        public virtual Rider Rider { get; set; }
    }
}
