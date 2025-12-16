using Microsoft.AspNetCore.SignalR;

namespace MESAmetrics.Hubs
{
    public class MachineHub : Hub
    {
        public async Task JoinGroup(string realTimeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, realTimeId);
        }
    }
}