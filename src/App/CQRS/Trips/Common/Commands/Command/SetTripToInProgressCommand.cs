﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Trips.Common.Commands.Command
{
    public sealed class SetTripToInProgressCommand : AbstractCommand
    {
        public string TripId { get; }
        public string Token { get; }

        public SetTripToInProgressCommand(string correlationId, string tenantId, string userId,
            string tripId, string token)
            : base(correlationId, tenantId, userId)
        {
            TripId = tripId;
            Token = token;
        }
    }
}