using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Shared.Domain.Common;


namespace GestorEventosDeportivos.Modules.Carreras.Domain.Entities;

public class Evento : BaseEntity<Guid>, IAggregateRoot
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public uint CantidadParticipantes { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public bool RegistroHabilitado { get; set; } = true;
    public EstadoEvento EstadoEvento { get; set; } = EstadoEvento.SinComenzar;

    public List<Carrera> Carreras { get; set; } = new();
}

public class Carrera : BaseEntity<Guid>, IAggregateRoot
{
    public Guid EventoId { get; set; }
    public Evento? Evento { get; set; }
    public string? Ganador { get; set; }
    public TimeSpan? TiempoGanador { get; set; }
    public uint Longitud { get; set; }

    public List<Participacion> Participaciones { get; set; } = new();
    public List<PuntoDeControl> PuntosDeControl { get; set; } = new();
}

public class PuntoDeControl : BaseEntity<long>
{
    public uint Posicion { get; set; }
    public string Ubicacion { get; set; } = string.Empty;

    public Guid CarreraId { get; set; }
    public Carrera? Carrera { get; set; }
}

