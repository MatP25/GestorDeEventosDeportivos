using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Datatypes;

public record DatosParticipanteDTO
{
    public Guid ParticipanteId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Apellido { get; init; } = string.Empty;
    public uint? NumeroCorredor { get; init; }
    public uint Puesto { get; init; }
    public EstadoParticipanteEnCarrera Estado { get; init; }
}

public record ProgresoPuntoDeControlDTO
{
    public uint Posicion { get; init; }
    public string Ubicacion { get; init; } = string.Empty;
    // clave: participanteId, valor: tiempo
    public Dictionary<string, TimeSpan?> TiemposDeParticipantes { get; init; } = new();
}

public record ProgresoCarreraDTO
{
    public Guid CarreraId { get; init; }
    public string CarreraNombre { get; init; } = string.Empty;
    public uint CantidadParticipantes { get; init; }
    // clave: posicion del punto de control, valor: DTO del progreso en ese punto
    public Dictionary<uint, ProgresoPuntoDeControlDTO> ProgresoPorPuntosDeControl { get; init; } = new(); 
    public List<DatosParticipanteDTO> Participantes { get; init; } = new();
}