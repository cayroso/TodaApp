using Common.Extensions;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Queries.Query
{
    public sealed class SearchTripQuery : AbstractPagedQuery<SearchTripQuery.Trip>
    {
        public SearchTripQuery(string correlationId, string tenantId, string userId, string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
            : base(correlationId, tenantId, userId, criteria, pageIndex, pageSize, sortField, sortOrder)
        {

        }

        public class Trip
        {
            public string TripId { get; set; }

            public EnumTripStatus Status { get; set; }
            public string StatusText => Status.ToString();
            //public string CancelReason { get; set; }

            public Driver Driver { get; set; }

            public Rider Rider { get; set; }

            public string StartAddress { get; set; }
            //public string StartAddressDescription { get; set; }
            //public double StartX { get; set; }
            //public double StartY { get; set; }

            public string EndAddress { get; set; }
            //public string EndAddressDescription { get; set; }
            //public double EndX { get; set; }
            //public double EndY { get; set; }

            public decimal Fare { get; set; }

            public int RiderRating { get; set; }
            //public string RiderComment { get; set; }

            public int DriverRating { get; set; }
            //public string DriverComment { get; set; }


            DateTime _dateCreated;
            public DateTime DateCreated
            {
                get => _dateCreated;
                set => _dateCreated = value.AsUtc();
            }

            public string Token { get; set; }

            //public IEnumerable<TripTimeline> Timelines { get; set; } = new List<TripTimeline>();
            //public IEnumerable<TripLocation> Locations { get; set; } = new List<TripLocation>();

        }

        public class Driver
        {
            public string DriverId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Rider
        {
            public string RiderId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }


        public class TripTimeline
        {
            public EnumTripStatus Status { get; set; }
            public string StatusText => Status.ToString();
            public string Notes { get; set; }

            DateTime _dateTimeline;
            public DateTime DateTimeline
            {
                get => _dateTimeline;
                set => _dateTimeline = value.AsUtc();
            }
        }

        public class TripLocation
        {
            public EnunTripLocationType TripLocationType { get; set; }
            public string TripLocationTypeText => TripLocationType.ToString();

            public double GeoX { get; set; }
            public double GeoY { get; set; }

            DateTime _dateCreated;
            public DateTime DateCreated
            {
                get => _dateCreated;
                set => _dateCreated = value.AsUtc();
            }
        }
    }
}
