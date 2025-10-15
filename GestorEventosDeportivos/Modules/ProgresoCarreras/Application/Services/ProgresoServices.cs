using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Datatypes;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Application;

public class ProgresoServices : IProgresoService
{
    private readonly AppDbContext _context;

    public ProgresoServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Participacion> VerProgresoDeParticipanteEnCarrera(Guid carreraId, Guid participanteId)
    {
        Participacion? participacion = await _context.Participaciones.FirstOrDefaultAsync(p => p.EventoId == carreraId && p.ParticipanteId == participanteId);
        if (participacion == null)
            throw new NotFoundException($"No se encontró la participación del usuario de Id {participanteId} en la carrera {carreraId}.");

        return participacion;
    }

    public async Task<ProgresoCarreraDTO> VerProgresoGeneralDeCarrera(Guid carreraId)
    {
        Carrera carrera = await _context.Carreras
            .Include(c => c.Evento)
            .Include(c => c.Participaciones)
            .ThenInclude(p => p.Participante)
            .Include(c => c.PuntosDeControl)
            .FirstOrDefaultAsync(c => c.EventoId == carreraId)
            ?? throw new NotFoundException($"No se encontró la carrera con Id {carreraId}.");

        Dictionary<uint, ProgresoPuntoDeControlDTO> progresoPorPuntos = new();

        foreach (PuntoDeControl pc in carrera.PuntosDeControl)
        {
            Dictionary<Guid, TimeSpan?> tiemposParticipantes = new();
            foreach (Participacion part in carrera.Participaciones)
            {
                tiemposParticipantes.Add(
                    part.ParticipanteId,
                    part.Progreso.ContainsKey(pc.Posicion) ? part.Progreso[pc.Posicion] : null
                );
            }
            ProgresoPuntoDeControlDTO progresoPC = new()
            {
                Posicion = pc.Posicion,
                Ubicacion = pc.Ubicacion,
                TiemposDeParticipantes = tiemposParticipantes
            };
            progresoPorPuntos.Add(pc.Posicion, progresoPC);
        }

        return new ProgresoCarreraDTO
        {
            CarreraId = carrera.Id,
            CarreraNombre = carrera.Evento!.Nombre,
            CantidadParticipantes = (uint)carrera.Participaciones.Count,
            ProgresoPorPuntosDeControl = progresoPorPuntos,
            Participantes = carrera.Participaciones.Select(p => new DatosParticipanteDTO
            {
                ParticipanteId = p.ParticipanteId,
                Nombre = p.Participante!.Nombre,
                Apellido = p.Participante.Apellido,
                NumeroCorredor = p.NumeroCorredor,
                Puesto = p.Puesto,
                Estado = p.Estado
            }).ToList()
        };
    }


    public async Task IngresarLecturaPuntoDeControl(Guid carreraId, Guid participanteId, uint puntoDeControlPosicion, TimeSpan tiempo)
    {
        Participacion? participacion = await _context.Participaciones.FirstOrDefaultAsync(p => p.EventoId == carreraId && p.ParticipanteId == participanteId);
        if (participacion == null) 
            throw new NotFoundException($"No se encontró la participación del usuario de Id {participanteId} en la carrera {carreraId}.");

        participacion.Progreso.Add(puntoDeControlPosicion, tiempo);

        await _context.SaveChangesAsync();
    }
}