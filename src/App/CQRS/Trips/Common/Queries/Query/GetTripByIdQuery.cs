using Common.Extensions;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cayent.Core.CQRS.Queries;

namespace App.CQRS.Trips.Common.Queries.Query
{
    public sealed class GetTripByIdQuery : AbstractQuery<GetTripByIdQuery.Trip>
    {
        public string TripId { get; }

        public GetTripByIdQuery(string correlationId, string tenantId, string userId,
            string tripId)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
        }

        public class Trip
        {
            public string TripId { get; set; }

            public EnumTripStatus Status { get; set; }
            public string StatusText => Status.ToString();
            public string CancelReason { get; set; }

            public Rider Rider { get; set; }
            public Driver Driver { get; set; }
            public Vehicle Vehicle { get; set; }

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


            DateTime _dateCreated;
            public DateTime DateCreated
            {
                get => _dateCreated;
                set => _dateCreated = value.AsUtc();
            }

            public string Token { get; set; }

            public IEnumerable<TripTimeline> Timelines { get; set; } = new List<TripTimeline>();
            public IEnumerable<TripLocation> Locations { get; set; } = new List<TripLocation>();
            public IEnumerable<ExcludedDriver> ExcludedDrivers { get; set; } = new List<ExcludedDriver>();

        }

        public class Rider
        {
            public string RiderId { get; set; }
            public string UrlProfilePicture { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public double OverallRating { get; set; }
        }

        public class Driver
        {
            public string DriverId { get; set; }
            public string UrlProfilePicture { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public double OverallRating { get; set; }
        }

        public class ExcludedDriver
        {
            public string DriverId { get; set; }
            public string UrlProfilePicture { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string RejectReason { get; set; }
            public double OverallRating { get; set; }
        }

        public class Vehicle
        {
            public string VehicleId { get; set; }
            public string PlateNumber { get; set; }
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
