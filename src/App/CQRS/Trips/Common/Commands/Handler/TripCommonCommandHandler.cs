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
        ICommandHandler<DriverAcceptRiderTripRequestCommand>,
        ICommandHandler<DriverOfferFareToRiderTripRequestCommand>,
        ICommandHandler<DriverRejectRiderTripRequestCommand>,
        ICommandHandler<RiderAcceptDriverOfferCommand>,
        ICommandHandler<RiderCancelTripCommand>,
        ICommandHandler<RiderCreateTripCommand>,
        ICommandHandler<RiderRejectDriverOfferCommand>,
        ICommandHandler<RiderRequestTripCommand>,
        ICommandHandler<SetTripToCompleteCommand>,
        ICommandHandler<SetTripToInProgressCommand>

    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;

        public TripCommonCommandHandler(AppDbContext appDbContext, ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        #region Driver

        async Task ICommandHandler<DriverAcceptRiderTripRequestCommand>.HandleAsync(DriverAcceptRiderTripRequestCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.DriverAccepted;
            trip.Fare = 0;
            trip.Driver.Availability = EnumDriverAvailability.Unavailable;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<DriverOfferFareToRiderTripRequestCommand>.HandleAsync(DriverOfferFareToRiderTripRequestCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.DriverOfferedFare;
            trip.Fare = command.Fare;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<DriverRejectRiderTripRequestCommand>.HandleAsync(DriverRejectRiderTripRequestCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.DriverRejected;
            trip.Fare = 0;
            trip.Driver.Availability = EnumDriverAvailability.Available;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }


        #endregion

        #region Rider

        async Task ICommandHandler<RiderAcceptDriverOfferCommand>.HandleAsync(RiderAcceptDriverOfferCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.RiderOfferedFareAccepted;
            trip.Driver.Availability = EnumDriverAvailability.Unavailable;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderCancelTripCommand>.HandleAsync(RiderCancelTripCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.Cancelled;

            if (trip.Driver != null)
            {
                trip.Driver.Availability = EnumDriverAvailability.Available;
            }

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderCreateTripCommand>.HandleAsync(RiderCreateTripCommand command)
        {
            //  check if user has pending trip
            var existingTrip = await _appDbContext.Trips.Where(e => e.RiderId == command.UserId
                        && !(e.Status == EnumTripStatus.Complete || e.Status == EnumTripStatus.Cancelled)).AnyAsync();

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

                Status = EnumTripStatus.Pending,
            };

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
            });

            await _appDbContext.AddAsync(trip);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderRejectDriverOfferCommand>.HandleAsync(RiderRejectDriverOfferCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.RiderOfferedFareRejected;
            trip.Fare = 0;
            trip.Driver.Availability = EnumDriverAvailability.Available;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
                Notes = command.Notes
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<RiderRequestTripCommand>.HandleAsync(RiderRequestTripCommand command)
        {
            var trip = await _appDbContext.Trips.FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.Requested;
            trip.Fare = 0;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status,
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<SetTripToCompleteCommand>.HandleAsync(SetTripToCompleteCommand command)
        {

            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.Complete;
            trip.Driver.Availability = EnumDriverAvailability.Available;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status
            });

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<SetTripToInProgressCommand>.HandleAsync(SetTripToInProgressCommand command)
        {
            var trip = await _appDbContext.Trips.Include(e => e.Driver).FirstOrDefaultAsync(e => e.TripId == command.TripId);

            trip.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            trip.Status = EnumTripStatus.InProgress;
            trip.Driver.Availability = EnumDriverAvailability.Unavailable;

            trip.Timelines.Add(new TripTimeline
            {
                TripId = trip.TripId,
                UserId = command.UserId,
                Status = trip.Status
            });

            await _appDbContext.SaveChangesAsync();
        }



        #endregion

    }
}
