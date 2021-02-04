using App.CQRS.Trips.Common.Commands.Command;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Trips;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Handler
{
    public sealed class TripCommonCommandHandler :
        ICommandHandler<DriverAcceptedTripCommand>,
        ICommandHandler<DriverOfferFareCommand>,
        ICommandHandler<DriverRejectedTripCommand>,
        ICommandHandler<RiderCancelledTripCommand>,
        ICommandHandler<RiderCreatedTripCommand>,
        ICommandHandler<RiderRejectedDriverOfferCommand>,
        ICommandHandler<RiderRequestedTripCommand>

    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;

        public TripCommonCommandHandler(AppDbContext appDbContext, ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        #region Driver

        async Task ICommandHandler<DriverAcceptedTripCommand>.HandleAsync(DriverAcceptedTripCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.DriverAccepted;
            trip.Fare = 0;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<DriverOfferFareCommand>.HandleAsync(DriverOfferFareCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.DriverOfferedFare;
            trip.Fare = command.Fare;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
                Notes = $"Fare offered={command.Fare}"
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<DriverRejectedTripCommand>.HandleAsync(DriverRejectedTripCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.DriverRejected;
            trip.Fare = 0;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }


        #endregion

        #region Rider

        async Task ICommandHandler<RiderCancelledTripCommand>.HandleAsync(RiderCancelledTripCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.Requested;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderCreatedTripCommand>.HandleAsync(RiderCreatedTripCommand command)
        {
            //  check if user has pending trip
            var existingTrip = await _appDbContext.Trips.Where(e => e.RiderId == command.UserId
                        && !(e.Status == EnumRideStatus.Complete || e.Status == EnumRideStatus.Cancelled)).AnyAsync();

            if (existingTrip)
            {
                throw new ApplicationException("You have active trip.");
            }

            var trip = new Trip
            {
                TripId = command.TripId,
                RiderId = command.RiderId,

                StartAddress = command.StartAddress,
                StartAddressDescription = command.StartAddressDescription,
                StartX = command.StartX,
                StartY = command.StartY,

                EndAddress = command.EndAddress,
                EndAddressDescription = command.EndAddressDescription,
                EndX = command.EndX,
                EndY = command.EndY,

                Status = EnumRideStatus.Pending,
            };

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
            });

            await _appDbContext.AddAsync(trip);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderRejectedDriverOfferCommand>.HandleAsync(RiderRejectedDriverOfferCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.Requested;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderRequestedTripCommand>.HandleAsync(RiderRequestedTripCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumRideStatus.Requested;
            trip.Fare = 0;

            trip.Timelines.Add(new TripTimeline
            {
                Status = trip.Status,
            });

            await _appDbContext.SaveChangesAsync();
        }

        #endregion

    }
}
