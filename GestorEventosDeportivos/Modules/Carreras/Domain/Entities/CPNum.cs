using GestorEventosDeportivos.Shared.Domain.Common;

namespace GestorEventosDeportivos.Modules.Carreras.Domain.Entities;

// CarreraParticipanteNumero -> lleva el ultimo numero asignado por carrera
public class CPNum
{
    public Guid CarreraId { get; set; }
    public uint NextNumber { get; set; } = 0; // Ultimo numero asignado, el siguiente sera NextNumber + 1
}
