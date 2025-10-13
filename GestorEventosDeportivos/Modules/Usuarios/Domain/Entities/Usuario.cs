using GestorEventosDeportivos.Shared.Domain.Common;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;

namespace GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;

public class Usuario : BaseEntity<Guid>, IAggregateRoot{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime FechaNac { get; set; }
}

public class Administrador : Usuario {

}

public class Participante : Usuario {
    public List<Participacion> Carreras { get; set; } = new();

}