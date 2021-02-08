﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel.Trips
{
    #region Driver

    public class DriverAssigned
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class DriverAccepted
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class DriverRejected
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class DriverFareOffered
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public decimal Fare { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class DriverTripInProgress
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public decimal Fare { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class DriverTripCompleted
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public decimal Fare { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    #endregion

    #region Rider

    public class RiderTripRequested
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class RiderOfferedFareAccepted
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public decimal Fare { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class RiderOfferedFareRejected
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public decimal Fare { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class RiderTripCancelled
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
            public string Reason { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class RiderTripInProgress
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }

    public class RiderTripCompleted
    {
        public class Request
        {

        }

        public class RequestInfo
        {

        }

        public class Response
        {
            public string TripId { get; set; }
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string RiderId { get; set; }
            public string RiderName { get; set; }
        }

        public class ResponseInfo
        {

        }
    }
    #endregion


}
