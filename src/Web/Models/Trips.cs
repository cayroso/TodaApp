using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{

    #region Driver

    public class DriverAcceptRiderTripRequestInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
    }

    public class DriverRejectRiderTripRequestInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
        public string Notes { get; set; }
    }

    public class DriverOfferFareToRiderTripRequestInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
        public decimal Fare { get; set; }
        public string Notes { get; set; }
    }

    #endregion

    #region Rider

    public class RiderCreateTripInfo
    {
        public string StartAddress { get; set; }
        public string StartAddressDescription { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }

        public string EndAddress { get; set; }
        public string EndAddressDescription { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
    }

    public class RiderRequestTripInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
    }

    public class RiderCancelTripInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
        public string Notes { get; set; }
    }

    public class RiderAcceptDriverOfferInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
    }

    public class RiderRejectDriverOfferInfo
    {
        public string TripId { get; set; }
        public string Token { get; set; }
        public string Notes { get; set; }
    }

    #endregion


    public class AddFeedbackInfo
    {
        [Required]
        public string TripId { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
