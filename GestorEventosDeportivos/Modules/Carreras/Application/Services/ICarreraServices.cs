using System.Threading;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Shared.Domain.Common;
using GestorEventosDeportivos.Modules.Carreras.Application.Services.DTOs;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;

namespace GestorEventosDeportivos.Modules.Carreras.Application.Services;

public interface ICarreraService{
    Task<Evento> CrearEvento(string nombre, DateTime fechaInicio, uint Capacidad, string Ubicacion);
    Task<Carrera> CrearCarrera(Guid eventoId, uint longitud, IEnumerable<(uint posicion, string ubicacion)> puntosDeControl);
    Task<bool> AgregarParticipante(Guid eventoId, Guid participanteId);
    Task<bool> QuitarParticipante(Guid eventoId, Guid participanteId);
    Task<Evento?> HabilitarRegistro(Guid eventoId);
    Task<Evento?> DeshabilitarRegistro(Guid eventoId);
    Task<IEnumerable<Carrera>> ListarCarreras();
    Task<IEnumerable<Carrera>> ListarCarrerasPorFecha(DateTime fechaDesde, DateTime fechaHasta);
    Task<IEnumerable<Carrera>> ListarCarrerasPorUbicacion(string ubicacion);
    Task<PagedResult<Carrera>> ListarCarrerasPaginado(int page, int pageSize, bool ordenarAsc, IEnumerable<EstadoEvento>? estados = null);
    Task<CarreraResultados?> ObtenerResultadosCarrera(Guid carreraId);
    Task<Carrera?> ObtenerCarreraConDetalle(Guid carreraId);
    Task<PagedResult<Participacion>> ListarParticipacionesCarreraPaginado(Guid carreraId, int page, int pageSize, string? filtroNombre = null);
    Task<IEnumerable<Evento>> ListarEventos();
    Task<PuntoDeControl> AgregarPuntoAlaCarrera(Guid carreraId, uint posicion, string ubicacion);
    Task<Evento> RecalcularEstadoEvento(Guid eventoId);
    Task<IEnumerable<Participacion>> ListarParticipacionesDeUsuario(Guid usuarioId);
    Task<bool> ActualizarEstadoPagoParticipacion(Guid carreraId, Guid participanteId, EstadoPago nuevoEstadoPago);
}
