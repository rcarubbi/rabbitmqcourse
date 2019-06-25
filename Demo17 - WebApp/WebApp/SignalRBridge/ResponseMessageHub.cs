using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignalRBridge
{
    public class ResponseMessageHub : Hub
    {
        public override Task OnConnected()
        {
            Clients.Caller.GetConnectionId(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}
