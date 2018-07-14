using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using Microsoft.AspNet.Identity;

namespace UserTablesPrimer.Hubs
{
    public class SignalRHub : Hub
    {
        public static ConcurrentDictionary<string, string> connectedUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnected()
        {
            if (Context.User.Identity.Name == null)
                connectedUsers.TryAdd(Guid.NewGuid().ToString(), Context.ConnectionId);
            else
                connectedUsers.TryAdd(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string value;
            connectedUsers.TryRemove(Context.User.Identity.Name, out value);
            return base.OnDisconnected(stopCalled);
        }

        /*
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return Connection.Broadcast(data);
        }//*/
    }
}