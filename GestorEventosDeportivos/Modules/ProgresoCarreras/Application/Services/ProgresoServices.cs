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

    public async Task<Participacion> VerProgresoDeParticipanteEnCarrera(Guid eventoId, Guid participanteId)
    {
        Participacion? participacion = await _context.Participaciones.FirstOrDefaultAsync(p => p.EventoId == eventoId && p.ParticipanteId == participanteId);
        if (participacion == null)
            throw new NotFoundException($"No se encontró la participación del usuario de Id {participanteId} en el evento {eventoId}.");

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
        // Obtener la carrera por su Id
        var carrera = await _context.Carreras
            .Include(c => c.PuntosDeControl)
            .Include(c => c.Evento)
            .Include(c => c.Participaciones)
            .FirstOrDefaultAsync(c => c.Id == carreraId)
            ?? throw new NotFoundException($"No se encontró la carrera con Id {carreraId}.");

        // Buscar la participacion del participante dentro de la carrera
        Participacion? participacion = carrera.Participaciones.FirstOrDefault(p => p.ParticipanteId == participanteId);

        if (participacion == null)
            throw new NotFoundException($"No se encontró la participación del usuario de Id {participanteId} en la carrera {carreraId}.");

        // Registrar progreso en el punto indicado (si ya existe, actualiza el tiempo)
        participacion.Progreso[puntoDeControlPosicion] = tiempo;

        // Actualizar estado del participante segun el progreso
        var totalPuntos = carrera.PuntosDeControl.Count;
        if (totalPuntos > 0)
        {
            if (participacion.Estado == EstadoParticipanteEnCarrera.SinComenzar)
            {
                participacion.Estado = EstadoParticipanteEnCarrera.EnCurso;
            }

            // Si alcanzo el ultimo punto, marcar como Completada
            if (puntoDeControlPosicion >= totalPuntos)
            {
                participacion.Estado = EstadoParticipanteEnCarrera.Completada;

                // Asignar Puesto de llegada de forma segura (con bloqueo de fila de Carrera)
                if (participacion.Puesto == 0)
                {
                    using var tx = await _context.Database.BeginTransactionAsync();
                    // Bloquear la fila de la carrera para serializar la asignacion de puestos entre instancias
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1 FROM `Carreras` WHERE `Id` = {0} FOR UPDATE", carreraId);

                    // Obtener el maximo puesto asignado en esta carrera y asignar el siguiente
                    var maxPuesto = await _context.Participaciones
                        .Where(p => EF.Property<Guid?>(p, "CarreraId") == carreraId && p.Puesto > 0)
                        .MaxAsync(p => (uint?)p.Puesto) ?? 0;

                    if (participacion.Puesto == 0) // doble chequeo dentro de la transaccion
                    {
                        participacion.Puesto = maxPuesto + 1;
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();
                }
            }
        }

        await _context.SaveChangesAsync();

        // Recalcular estado del evento tras el cambio de estado del participante
        await _carreraService.RecalcularEstadoEvento(carrera.EventoId);
    }
    
    public async Task AbandonarCarrera(Guid carreraId,  Guid participanteId)
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

    public async Task DescalificarParticipante(Guid carreraId, Guid participanteId)
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
            throw new DomainRuleException("No se puede descalificar en una carrera que ya ha finalizado o que no ha comenzado.");

        participacion.Estado = EstadoParticipanteEnCarrera.Descalificado;
        await _context.SaveChangesAsync();
    }
}