using System.Threading;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;

namespace GestorEventosDeportivos.Modules.Carreras.Application.Services;

public interface ICarreraService{
    Task<Evento> CrearEvento(string nombre, DateTime fechaInicio, uint Capacidad, string Ubicacion, CancellationToken ctoken = default);
    Task<Carrera> CrearCarrera(Guid eventoId, uint longitud, IEnumerable<(uint posicion, string ubicacion)> puntosDeControl, CancellationToken ctoken = default);
    Task<bool> AgregarParticipante(Guid eventoId, Guid participanteId, CancellationToken ctoken = default);
    Task<bool> QuitarParticipante(Guid eventoId, Guid participanteId, CancellationToken ctoken = default);
    Task<Evento?> HabilitarRegistro(Guid eventoId, CancellationToken ctoken = default);
    Task<Evento?> DeshabilitarRegistro(Guid eventoId, CancellationToken ctoken = default);
    Task<IEnumerable<Carrera>> ListarCarreras(CancellationToken ctoken = default);
    Task<IEnumerable<Carrera>> ListarCarrerasPorFecha(DateTime fechaDesde, DateTime fechaHasta, CancellationToken ctoken = default);
    Task<IEnumerable<Carrera>> ListarCarrerasPorUbicacion(string ubicacion, CancellationToken ctoken = default);
}
