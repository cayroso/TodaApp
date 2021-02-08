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
        Task DriverAssigned(DriverAssigned.Response resp);
        Task DriverAccepted(DriverAccepted.Response resp);
        Task DriverRejected(DriverRejected.Response resp);
        Task DriverFareOffered(DriverFareOffered.Response resp);
        Task DriverTripInProgress(DriverTripInProgress.Response resp);
        Task DriverTripCompleted(DriverTripCompleted.Response resp);

        Task RiderTripRequested(RiderTripRequested.Response resp);
        Task RiderOfferedFareAccepted(RiderOfferedFareAccepted.Response resp);
        Task RiderOfferedFareRejected(RiderOfferedFareRejected.Response resp);
        Task RiderTripCancelled(RiderTripCancelled.Response resp);
        Task RiderTripInProgress(RiderTripInProgress.Response resp);
        Task RiderTripCompleted(RiderTripCompleted.Response resp);
    }

    [Authorize]
    public class TripHub : Hub<ITripClient>
    {

    }


}
