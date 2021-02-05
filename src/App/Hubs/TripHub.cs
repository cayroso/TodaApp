using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Trips;

namespace App.Hubs
{
    public interface ITripClient
    {
        Task TripRequested();
        Task TripCancelled();
        Task TripInProgress();
        Task TripCompleted();

        Task DriverAssigned(DriverAssigned.Response resp);
        Task DriverAccepted(DriverAccepted.Response resp);
        Task DriverRejected(DriverRejected.Response resp);
        Task DriverFareOffered(DriverFareOffered.Response resp);
        Task RiderOfferedFareAccepted();
        Task RiderOfferedFareRejected();










        //Task DriverAcceptedRiderTripRequest(DriverAcceptedRiderTripRequest.ResponseInfo info);
        //Task DriverOfferedFareToRiderTripRequest(DriverOfferedFareToRiderTripRequest.ResponseInfo info);
        //Task DriverRejectedRiderTripRequest(DriverRejectedRiderTripRequest.ResponseInfo info);

        //Task RiderTripCreated(RiderTripCreated.ResponseInfo info);
        //Task RiderTripRequested(RiderTripRequested.ResponseInfo info);
        //Task RiderTripCancelled(RiderTripCancelled.ResponseInfo info);
        //Task RiderAcceptedDriverOffer(RiderAcceptedDriverOffer.ResponseInfo info);
        //Task RiderRejectedDriverOffer(RiderRejectedDriverOffer.ResponseInfo info);
    }

    [Authorize]
    public class TripHub : Hub<ITripClient>
    {

    }


}
