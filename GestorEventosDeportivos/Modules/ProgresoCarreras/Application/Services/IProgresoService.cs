using GestorEventosDeportivos.Modules.ProgresoCarreras.Datatypes;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Application;

public interface IProgresoService
{
    Task<Participacion> VerProgresoDeParticipanteEnCarrera(Guid eventoId, Guid participanteId);
    Task<ProgresoCarreraDTO> VerProgresoGeneralDeCarrera(Guid carreraId);
    Task IngresarLecturaPuntoDeControl(Guid carreraId, Guid participanteId, uint puntoDeControlPosicion, TimeSpan tiempo);
    Task AbandonarCarrera(Guid carreraId,  Guid participanteId);
}