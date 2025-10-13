using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Shared.Domain.Common;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;


public class Participacion : BaseEntity<Guid>
{
    public Guid EventoId { get; set; }
    public Evento? Evento { get; set; }
    
    public Guid ParticipanteId { get; set; }
    public Participante? Participante { get; set; }

    public uint NumeroCorredor { get; set; }
    public uint Puesto { get; set; }
    public EstadoParticipanteEnCarrera Estado { get; set; } = EstadoParticipanteEnCarrera.SinComenzar;

    // progreso -> ([uint] clave) seria el numero de punto de control (posicion) y ([TimeSpan] valor) = Tiempo en el que paso por ese punto
    public Dictionary<uint, TimeSpan> Progreso { get; set; } = new();
}