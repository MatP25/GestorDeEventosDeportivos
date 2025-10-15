using GestorEventosDeportivos.Shared.Domain.Common;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;

namespace GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;

public class Usuario : BaseEntity<Guid>, IAggregateRoot{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly FechaNac { get; set; }

    public PerfilUsuarioDTO DatosPerfil() => new(Id, Nombre, Apellido, Email, FechaNac);
}

public class Administrador : Usuario {

}

public class Participante : Usuario
{
    public List<Participacion> Carreras { get; set; } = new();

}

public record PerfilUsuarioDTO(Guid Id, string Nombre, string Apellido, string Email, DateOnly FechaNac);