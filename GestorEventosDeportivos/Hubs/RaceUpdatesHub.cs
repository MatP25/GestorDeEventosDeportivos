using Microsoft.AspNetCore.SignalR;

namespace GestorEventosDeportivos.Hubs;

public class RaceUpdatesHub : Hub
{
    public async Task JoinRaceGroup(string raceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, raceId);
    }
}
