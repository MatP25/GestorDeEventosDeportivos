using System.Threading;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;

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
}
