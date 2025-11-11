using Microsoft.AspNetCore.SignalR;

namespace GestorEventosDeportivos.Hubs;

public class RaceHub : Hub
{
    // Cliente se une al grupo de la carrera 
    public Task JoinRace(string carreraId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, GroupName(carreraId));
    }

    public Task LeaveRace(string carreraId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(carreraId));
    }

    // Método que puede llamar el servidor para notificar progreso
    public Task BroadcastProgreso(string carreraId, Guid participanteId, uint posicion)
    {
        return Clients.Group(GroupName(carreraId)).SendAsync("ProgresoActualizado", participanteId, posicion);
    }

    private static string GroupName(string carreraId) => $"race-{carreraId}";
}