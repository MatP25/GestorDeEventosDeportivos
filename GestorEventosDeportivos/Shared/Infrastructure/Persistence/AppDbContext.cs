using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GestorEventosDeportivos.Shared.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Participante> Participantes => Set<Participante>();

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<PuntoDeControl> PuntosDeControl => Set<PuntoDeControl>();
    public DbSet<Participacion> Participaciones => Set<Participacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().HasDiscriminator<string>("TipoUsuario")
            .HasValue<Usuario>("Usuario")
            .HasValue<Administrador>("Admin")
            .HasValue<Participante>("Participante");

        modelBuilder.Entity<Carrera>()
            .HasOne(c => c.Evento)
            .WithMany(e => e.Carreras)
            .HasForeignKey(c => c.EventoId);

        modelBuilder.Entity<PuntoDeControl>()
            .HasOne(p => p.Carrera)
            .WithMany(c => c.PuntosDeControl)
            .HasForeignKey(p => p.CarreraId);

        modelBuilder.Entity<Participacion>()
            .HasOne(p => p.Evento)
            .WithMany()
            .HasForeignKey(p => p.EventoId);

        modelBuilder.Entity<Participacion>()
            .HasOne(p => p.Participante)
            .WithMany(pp => pp.Carreras)
            .HasForeignKey(p => p.ParticipanteId);

        modelBuilder.Entity<Participacion>()
            .Property(p => p.Progreso)
            .HasConversion(
                v => string.Join(";", v.Select(kvp => $"{kvp.Key}:{kvp.Value}")),
                v => v
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split(new[] { ':' }, StringSplitOptions.None))
                    .ToDictionary(
                        arr => uint.Parse(arr[0], CultureInfo.InvariantCulture),
                        arr => TimeSpan.Parse(arr[1], CultureInfo.InvariantCulture)
                    )
            );
    }
}
