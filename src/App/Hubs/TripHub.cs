using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Hubs
{
    public interface ITripClient
    {

    }

    [Authorize]
    public class TripHub : Hub<ITripClient>
    {
    }
}
