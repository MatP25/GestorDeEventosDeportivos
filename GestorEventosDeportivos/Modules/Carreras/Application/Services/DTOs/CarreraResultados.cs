using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;

namespace GestorEventosDeportivos.Modules.Carreras.Application.Services.DTOs;

public class CarreraResultados
{
    public required Carrera Carrera { get; init; }
    public required List<Participacion> Participaciones { get; init; }
}
