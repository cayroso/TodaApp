using App.CQRS.Trips.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Queries.Handler
{
    public sealed class TripCommonQueryHandler :
        IQueryHandler<GetTripByIdQuery, GetTripByIdQuery.Trip>,
        IQueryHandler<SearchTripQuery, Paged<SearchTripQuery.Trip>>
    {
        readonly AppDbContext _appDbContext;
        public TripCommonQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        async Task<GetTripByIdQuery.Trip> IQueryHandler<GetTripByIdQuery, GetTripByIdQuery.Trip>.HandleAsync(GetTripByIdQuery query)
        {
            var sql = from t in _appDbContext.Trips.AsNoTracking()

                      where t.TripId == query.TripId

                      select new GetTripByIdQuery.Trip
                      {
                          TripId = t.TripId,
                          Status = t.Status,
                          CancelReason = t.CancelReason,

                          Driver = t.Driver == null ? null : new GetTripByIdQuery.Driver
                          {
                              DriverId = t.Driver.DriverId,
                              UrlProfilePicture = t.Driver.User.Image == null ? null : t.Driver.User.Image.Url,
                              FirstName = t.Driver.User.FirstName,
                              MiddleName = t.Driver.User.MiddleName,
                              LastName = t.Driver.User.LastName,
                              PhoneNumber = t.Driver.User.PhoneNumber
                          },
                          Vehicle = t.Vehicle == null ? null : new GetTripByIdQuery.Vehicle
                          {
                              VehicleId = t.Vehicle.VehicleId,
                              PlateNumber = t.Vehicle.PlateNumber,
                          },
                          Rider = new GetTripByIdQuery.Rider
                          {
                              RiderId = t.Rider.RiderId,
                              UrlProfilePicture = t.Rider.User.Image == null ? null : t.Rider.User.Image.Url,
                              FirstName = t.Rider.User.FirstName,
                              MiddleName = t.Rider.User.MiddleName,
                              LastName = t.Rider.User.LastName,
                              PhoneNumber = t.Rider.User.PhoneNumber
                          },

                          StartAddress = t.StartAddress,
                          StartAddressDescription = t.StartAddressDescription,
                          StartX = t.StartX,
                          StartY = t.StartY,
                          EndAddress = t.EndAddress,
                          EndAddressDescription = t.EndAddressDescription,
                          EndX = t.EndX,
                          EndY = t.EndY,

                          DriverRating = t.DriverRating,
                          DriverComment = t.DriverComment,

                          RiderRating = t.RiderRating,
                          RiderComment = t.RiderComment,

                          Fare = t.Fare,
                          DateCreated = t.DateCreated,

                          Token = t.ConcurrencyToken,

                          Timelines = t.Timelines.OrderBy(e => e.DateTimeline).Select(e => new GetTripByIdQuery.TripTimeline
                          {
                              Status = e.Status,
                              Notes = e.Notes,
                              DateTimeline = e.DateTimeline
                          }),

                          Locations = t.Locations.OrderBy(e => e.DateCreated).Select(e => new GetTripByIdQuery.TripLocation
                          {
                              TripLocationType = e.TripLocationType,
                              GeoX = e.GeoX,
                              GeoY = e.GeoY,
                              DateCreated = e.DateCreated
                          }),

                          ExcludedDrivers = t.ExcludedDrivers.Select(e => new GetTripByIdQuery.ExcludedDriver
                          {
                              DriverId = e.Driver.DriverId,
                              UrlProfilePicture = e.Driver.User.Image.Url,
                              FirstName = e.Driver.User.FirstName,
                              MiddleName = e.Driver.User.MiddleName,
                              LastName = e.Driver.User.LastName,
                              PhoneNumber = e.Driver.User.PhoneNumber,
                              RejectReason = e.Reason,
                          })

                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchTripQuery.Trip>> IQueryHandler<SearchTripQuery, Paged<SearchTripQuery.Trip>>.HandleAsync(SearchTripQuery query)
        {
            var sql = from t in _appDbContext.Trips.AsNoTracking()

                      where string.IsNullOrWhiteSpace(query.UserId) || t.DriverId == query.UserId || t.RiderId == query.UserId

                      select new SearchTripQuery.Trip
                      {
                          TripId = t.TripId,
                          Status = t.Status,
                          //CancelReason = t.CancelReason,

                          Driver = t.Driver == null ? null : new SearchTripQuery.Driver
                          {
                              DriverId = t.Driver.DriverId,
                              UrlProfilePicture = t.Driver.User.Image == null ? null : t.Driver.User.Image.Url,
                              FirstName = t.Driver.User.FirstName,
                              MiddleName = t.Driver.User.MiddleName,
                              LastName = t.Driver.User.LastName,
                              PhoneNumber = t.Driver.User.PhoneNumber
                          },

                          Rider = new SearchTripQuery.Rider
                          {
                              RiderId = t.Rider.RiderId,
                              UrlProfilePicture = t.Rider.User.Image == null ? null : t.Rider.User.Image.Url,
                              FirstName = t.Rider.User.FirstName,
                              MiddleName = t.Rider.User.MiddleName,
                              LastName = t.Rider.User.LastName,
                              PhoneNumber = t.Rider.User.PhoneNumber,
                          },

                          StartAddress = t.StartAddress,
                          //StartAddressDescription = t.StartAddressDescription,
                          //StartX = t.StartX,
                          //StartY = t.StartY,
                          EndAddress = t.EndAddress,
                          //EndAddressDescription = t.EndAddressDescription,
                          //EndX = t.EndX,
                          //EndY = t.EndY,

                          DriverRating = t.DriverRating,
                          //DriverComment = t.DriverComment,

                          RiderRating = t.RiderRating,
                          //RiderComment = t.RiderComment,

                          Fare = t.Fare,
                          DateCreated = t.DateCreated,

                          Token = t.ConcurrencyToken,

                          //Timelines = t.Timelines.OrderBy(e => e.DateTimeline).Select(e => new SearchTripQuery.TripTimeline
                          //{
                          //    Status = e.Status,
                          //    Notes = e.Notes,
                          //    DateTimeline = e.DateTimeline
                          //}),

                          //Locations = t.Locations.OrderBy(e => e.DateCreated).Select(e => new SearchTripQuery.TripLocation
                          //{
                          //    TripLocationType = e.TripLocationType,
                          //    GeoX = e.GeoX,
                          //    GeoY = e.GeoY,
                          //    DateCreated = e.DateCreated
                          //})
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

            return dto;
        }
    }
}
