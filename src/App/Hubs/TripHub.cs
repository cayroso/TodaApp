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
    public class Response
    {
        public string TripId { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string RiderId { get; set; }
        public string RiderName { get; set; }
        public decimal Fare { get; set; }
        public string Reason { get; set; }
    }

    public interface ITripClient
    {
        Task DriverAssigned(Response resp);
        Task DriverAccepted(Response resp);
        Task DriverRejected(Response resp);
        Task DriverFareOffered(Response resp);
        Task DriverTripInProgress(Response resp);
        Task DriverTripCompleted(Response resp);

        Task RiderTripRequested(Response resp);
        Task RiderOfferedFareAccepted(Response resp);
        Task RiderOfferedFareRejected(Response resp);
        Task RiderTripCancelled(Response resp);
        Task RiderTripInProgress(Response resp);
        Task RiderTripCompleted(Response resp);
    }

    [Authorize]
    public class TripHub : Hub<ITripClient>
    {

    }


}
