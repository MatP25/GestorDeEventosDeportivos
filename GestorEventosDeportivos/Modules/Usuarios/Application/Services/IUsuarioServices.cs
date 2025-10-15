using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;

namespace GestorEventosDeportivos.Modules.Usuarios.Application.Services;

public interface IUsuarioServices
{
    Task<Usuario> RegistrarUsuarioParticipante(string Nombre, string Apellido, string Email, string Password, DateOnly fechaNac);
    Task<PerfilUsuarioDTO> ObtenerDatosUsuarioPorEmail(string Email);
    Task<PerfilUsuarioDTO> ObtenerDatosUsuarioId(Guid usuarioId);
    Task<IEnumerable<Participacion>> ObtenerParticipacionesDeUsuario(Guid usuarioId);
}