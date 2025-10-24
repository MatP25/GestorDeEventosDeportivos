using Microsoft.AspNetCore.SignalR;

namespace GestorEventosDeportivos.Hubs;

public class VerificationHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}
