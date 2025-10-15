using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.Usuarios.Application.Services;

public class UsuarioServices : IUsuarioServices
{
    private readonly AppDbContext _context;

    public UsuarioServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> RegistrarUsuarioParticipante(string Nombre, string Apellido, string Email, string Password, DateOnly fechaNac)
    {
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidDataException("Todos los campos son obligatorios.");
        }


        Usuario? existente = _context.Usuarios.FirstOrDefault(u => u.Email == Email);
        if (existente is not null)
        {
            throw new DuplicateException($"El email {Email} ya está registrado.");
        }

        Participante nuevo = new()
        {
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Password = Password,
            FechaNac = fechaNac
        };        
        _context.Usuarios.Add(nuevo);
        await _context.SaveChangesAsync();
        return nuevo;
    }

    public async Task<PerfilUsuarioDTO> ObtenerDatosUsuarioPorEmail(string Email)
    {
        Usuario? usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == Email);
        if (usuario is null)
        {
            throw new NotFoundException($"No se encontró un usuario con el email {Email}.");
        }

        return usuario.DatosPerfil();
    }

    public async Task<PerfilUsuarioDTO> ObtenerDatosUsuarioId(Guid usuarioId)
    {
        Usuario? usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new NotFoundException($"No se encontró un usuario con el ID {usuarioId}.");
        }

        return usuario.DatosPerfil();
    }

    public async Task<IEnumerable<Participacion>> ObtenerParticipacionesDeUsuario(Guid usuarioId)
    {
        IEnumerable<Participacion> participaciones = await _context.Participaciones
            .Include(p => p.Evento)
            .Where(p => p.ParticipanteId == usuarioId)
            .ToListAsync();

        return participaciones;
    }
}   