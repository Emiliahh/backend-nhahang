using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public class AdminHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
