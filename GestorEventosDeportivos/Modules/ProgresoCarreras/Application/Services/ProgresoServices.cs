using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Application.Services;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Datatypes;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Application;

public class ProgresoServices : IProgresoService
{
    private readonly AppDbContext _context;
    private readonly ICarreraService _carreraService;

    public ProgresoServices(AppDbContext context, ICarreraService carreraService)
    {
        _context = context;
        _carreraService = carreraService;
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
            .FirstOrDefaultAsync(c => c.Id == carreraId)
            ?? throw new NotFoundException($"No se encontró la carrera con Id {carreraId}.");

        Dictionary<uint, ProgresoPuntoDeControlDTO> progresoPorPuntos = new();

        foreach (PuntoDeControl pc in carrera.PuntosDeControl)
        {
            Dictionary<string, TimeSpan?> tiemposParticipantes = new();
            foreach (Participacion part in carrera.Participaciones)
            {
                tiemposParticipantes.Add(
                    string.Concat("#", part.NumeroCorredor, " ", part.Participante!.Nombre, " ", part.Participante.Apellido),
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

        // Registrar progreso en el punto indicado (si ya existe, actualiza el tiempo)
        participacion.Progreso[puntoDeControlPosicion] = tiempo;

        // Actualizar estado del participante según el progreso
        // Obtener la cantidad total de puntos de control de la carrera (por evento)
        var carrera = await _context.Carreras
            .Include(c => c.PuntosDeControl)
            .FirstOrDefaultAsync(c => c.EventoId == carreraId);

        if (carrera != null)
        {
            var totalPuntos = carrera.PuntosDeControl.Count;

            if (totalPuntos > 0)
            {
                // Si es la primera lectura y estaba SinComenzar, pasa a EnCurso
                if (participacion.Estado == EstadoParticipanteEnCarrera.SinComenzar)
                {
                    participacion.Estado = EstadoParticipanteEnCarrera.EnCurso;
                }

                // Si alcanzó el último punto, marcar como Completada
                if (puntoDeControlPosicion >= totalPuntos)
                {
                    participacion.Estado = EstadoParticipanteEnCarrera.Completada;
                }
            }
        }

        await _context.SaveChangesAsync();

        // Recalcular estado del evento tras el cambio de estado del participante
        await _carreraService.RecalcularEstadoEvento(carreraId);
    }

    public async Task AbandonarCarrera(Guid carreraId, Guid participanteId)
    {
        Carrera? carrera = await _context.Carreras
            .Include(c => c.Evento)
            .Include(c => c.Participaciones)
            .FirstOrDefaultAsync(c => c.Id == carreraId);

        if (carrera == null)
            throw new NotFoundException($"No se encontró la carrera con Id {carreraId}.");

        Participacion? participacion = carrera.Participaciones.FirstOrDefault(p => p.ParticipanteId == participanteId);
        if (participacion == null)
            throw new NotFoundException($"No se encontró la participación del usuario con Id {participanteId} en la carrera {carreraId}.");

        if (carrera.Evento!.EstadoEvento == EstadoEvento.Finalizado
            || carrera.Evento!.EstadoEvento == EstadoEvento.SinComenzar)
            throw new DomainRuleException("No se puede abandonar una carrera que ya ha finalizado o que no ha comenzado.");

        participacion.Estado = EstadoParticipanteEnCarrera.Abandonada;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Participacion>> ListarParticipacionesConProgresoDeCarrera(Guid carreraId)
    {
        var carrera = await _context.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carreraId);
        return await _context.Participaciones
            .Where(p => p.EventoId == carrera.EventoId)
            .Include(p => p.Participante)
            .ToListAsync();
    }
}