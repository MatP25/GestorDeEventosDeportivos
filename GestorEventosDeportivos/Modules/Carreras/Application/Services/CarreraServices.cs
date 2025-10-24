using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using GestorEventosDeportivos.Shared.Domain.Common;
using GestorEventosDeportivos.Modules.Carreras.Application.Services.DTOs;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.Carreras.Application.Services;

public class CarreraService : ICarreraService
{
	private readonly AppDbContext _db;

	public CarreraService(AppDbContext db)
	{
		_db = db;
	}

	public async Task<Evento> CrearEvento(string nombre, DateTime fechaInicio, uint Capacidad, string Ubicacion)
	{
		var ev = new Evento
		{
			Nombre = nombre,
			FechaInicio = fechaInicio,
			CantidadParticipantes = Capacidad,
			Ubicacion = Ubicacion
		};
		_db.Eventos.Add(ev);
		await _db.SaveChangesAsync();
		return ev;
	}

	public async Task<Carrera> CrearCarrera(Guid eventoId, uint longitud, IEnumerable<(uint posicion, string ubicacion)> puntosDeControl)
	{
		var carrera = new Carrera
		{
			EventoId = eventoId,
			Longitud = longitud,
			PuntosDeControl = puntosDeControl
				.Select(pc => new PuntoDeControl { Posicion = pc.posicion, Ubicacion = pc.ubicacion })
				.ToList()
		};
		_db.Carreras.Add(carrera);
		await _db.SaveChangesAsync();
		return carrera;
	}

	public Task<bool> AgregarParticipante(Guid eventoId, Guid participanteId)
	{
		throw new NotImplementedException();
	}

	public Task<bool> QuitarParticipante(Guid eventoId, Guid participanteId)
	{
		throw new NotImplementedException();
	}

	public async Task<Evento?> HabilitarRegistro(Guid eventoId)
	{
		var ev = await _db.Eventos.FindAsync(eventoId);
		if (ev is null) return null;
		ev.RegistroHabilitado = true;
		await _db.SaveChangesAsync();
		return ev;
	}

	public async Task<Evento?> DeshabilitarRegistro(Guid eventoId)
	{
		var ev = await _db.Eventos.FindAsync(eventoId);
		if (ev is null) return null;
		ev.RegistroHabilitado = false;
		await _db.SaveChangesAsync();
		return ev;
	}

	public async Task<IEnumerable<Carrera>> ListarCarreras()
	{
		return await _db.Carreras
			.Include(c => c.Evento)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<IEnumerable<Evento>> ListarEventos()
	{
		return await _db.Eventos.AsNoTracking().OrderBy(e => e.FechaInicio).ToListAsync();
	}

	public async Task<IEnumerable<Carrera>> ListarCarrerasPorFecha(DateTime fechaDesde, DateTime fechaHasta)
	{
		return await _db.Carreras
			.Include(c => c.Evento)
			.Where(c => c.Evento != null && c.Evento.FechaInicio >= fechaDesde && c.Evento.FechaInicio <= fechaHasta)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<IEnumerable<Carrera>> ListarCarrerasPorUbicacion(string ubicacion)
	{
		return await _db.Carreras
			.Include(c => c.Evento)
			.Where(c => c.Evento != null && EF.Functions.Like(c.Evento.Ubicacion, $"%{ubicacion}%"))
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<PagedResult<Carrera>> ListarCarrerasPaginado(int page, int pageSize, bool ordenarAsc, IEnumerable<EstadoEvento>? estados = null)
	{
		if (page < 1) page = 1;
		if (pageSize < 1) pageSize = 10;

		var q = _db.Carreras
			.Include(c => c.Evento)
			.AsNoTracking()
			.AsQueryable();

		if (estados != null)
		{
			var estadosList = estados.ToList();
			if (estadosList.Count == 0)
			{
				return new PagedResult<Carrera>
				{
					Items = Array.Empty<Carrera>(),
					TotalCount = 0,
					Page = page,
					PageSize = pageSize
				};
			}
			q = q.Where(c => c.Evento != null && estadosList.Contains(c.Evento!.EstadoEvento));
		}

		q = ordenarAsc
			? q.OrderBy(c => c.Evento!.FechaInicio)
			: q.OrderByDescending(c => c.Evento!.FechaInicio);

		var total = await q.CountAsync();
		var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

		return new PagedResult<Carrera>
		{
			Items = items,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}

	public async Task<CarreraResultados?> ObtenerResultadosCarrera(Guid carreraId)
	{
		var carrera = await _db.Carreras
			.Include(c => c.Evento)
			.Include(c => c.PuntosDeControl)
			.FirstOrDefaultAsync(c => c.Id == carreraId);

		if (carrera is null)
			return null;

		var participaciones = await _db.Participaciones
			.Include(p => p.Participante)
			.Where(p => p.EventoId == carrera.EventoId)
			.AsNoTracking()
			.ToListAsync();

		return new CarreraResultados
		{
			Carrera = carrera,
			Participaciones = participaciones
		};
	}

	public async Task<Carrera?> ObtenerCarreraConDetalle(Guid carreraId)
	{
		return await _db.Carreras
			.Include(c => c.Evento)
			.Include(c => c.PuntosDeControl)
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == carreraId);
	}

	public async Task<PuntoDeControl> AgregarPuntoAlaCarrera(Guid carreraId, uint posicion, string ubicacion)
	{
		var carrera = await _db.Carreras.Include(c => c.PuntosDeControl).FirstOrDefaultAsync(c => c.Id == carreraId);
		if (carrera is null) throw new NotFoundException("Carrera no encontrada");

		if (carrera.PuntosDeControl.Any(p => p.Posicion == posicion))
		{
			throw new DuplicateException($"Ya existe un punto en la posición {posicion} para esta carrera.");
		}

		var pc = new PuntoDeControl
		{
			Id = 0, // EF generará el id si es identity o se usará el tipo long; dejamos 0 para que EF lo asigne
			CarreraId = carreraId,
			Posicion = posicion,
			Ubicacion = ubicacion
		};

		_db.PuntosDeControl.Add(pc);
		await _db.SaveChangesAsync();
		return pc;
	}

	public async Task<Evento> RecalcularEstadoEvento(Guid eventoId)
	{
		var ev = await _db.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId);
		if (ev is null) throw new NotFoundException("Evento no encontrado");

		var participaciones = await _db.Participaciones
			.Where(p => p.EventoId == eventoId)
			.Select(p => p.Estado)
			.ToListAsync();

		var nuevoEstado = CalcularEstadoEventoDesdeParticipaciones(participaciones);
		if (ev.EstadoEvento != nuevoEstado)
		{
			ev.EstadoEvento = nuevoEstado;
			await _db.SaveChangesAsync();
		}
		return ev;
	}

	private static EstadoEvento CalcularEstadoEventoDesdeParticipaciones(IEnumerable<EstadoParticipanteEnCarrera> estados)
	{
		// Regla: si hay alguna EnCurso => Evento EnCurso
		// si no hay EnCurso pero hay alguna SinComenzar => Evento SinComenzar
		// si no hay ni EnCurso ni SinComenzar => Evento Finalizado
		bool anyEnCurso = estados.Any(s => s == EstadoParticipanteEnCarrera.EnCurso);
		if (anyEnCurso) return EstadoEvento.EnCurso;

		bool anySinComenzar = estados.Any(s => s == EstadoParticipanteEnCarrera.SinComenzar);
		if (anySinComenzar) return EstadoEvento.SinComenzar;

		return EstadoEvento.Finalizado;
	}

	public async Task<PagedResult<Participacion>> ListarParticipacionesCarreraPaginado(Guid carreraId, int page, int pageSize)
	{
		if (page < 1) page = 1;
		if (pageSize < 1) pageSize = 10;

		var carrera = await _db.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carreraId);
		if (carrera == null)
		{
			return new PagedResult<Participacion>
			{
				Items = Array.Empty<Participacion>(),
				TotalCount = 0,
				Page = page,
				PageSize = pageSize
			};
		}

		var q = _db.Participaciones
			.Include(p => p.Participante)
			.Where(p => p.EventoId == carrera.EventoId)
			.AsNoTracking();

		// Orden por puesto (los 0 al final) y luego por número de corredor para estabilidad
		q = q.OrderBy(p => p.Puesto == 0 ? int.MaxValue : (int)p.Puesto)
			 .ThenBy(p => p.NumeroCorredor);

		var total = await q.CountAsync();
		var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

		return new PagedResult<Participacion>
		{
			Items = items,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}

	public async Task<IEnumerable<Participacion>> ListarParticipacionesDeUsuario(Guid usuarioId)
	{
		return await _db.Participaciones
			.Include(p => p.Evento)
			.Where(p => p.ParticipanteId == usuarioId)
			.AsNoTracking()
			.OrderByDescending(p => p.Evento != null ? p.Evento.FechaInicio : DateTime.MinValue)
			.ToListAsync();
	}
}