using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using GestorEventosDeportivos.Hubs;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Application;

// Observa cambios en la BD (participaciones/progreso) y emite "RaceUpdated" por SignalR
public class RaceWatcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<RaceUpdatesHub> _hub;
    private readonly ILogger<RaceWatcherService> _logger;
    private readonly int _intervaloSegundos;

    // Estado por carrera (hash de snapshot actual)
    private readonly ConcurrentDictionary<Guid, string> _hashPorCarrera = new();

    public RaceWatcherService(
        IServiceProvider serviceProvider,
        IHubContext<RaceUpdatesHub> hub,
        ILogger<RaceWatcherService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _hub = hub;
        _logger = logger;
        _intervaloSegundos = configuration.GetValue<int>("RaceWatcher:IntervaloSegundos", 1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RaceWatcher iniciado. Intervalo: {Intervalo}s", _intervaloSegundos);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScanAndNotifyAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // apagando
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en RaceWatcher");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_intervaloSegundos), stoppingToken);
            }
            catch (OperationCanceledException) { }
        }

        _logger.LogInformation("RaceWatcher detenido");
    }

    private async Task ScanAndNotifyAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Carreras cuyos eventos están en curso
        var carrerasEnCurso = await db.Carreras
            .Include(c => c.Evento)
            .Where(c => c.Evento!.EstadoEvento == EstadoEvento.EnCurso)
            .Select(c => new { c.Id, c.EventoId })
            .AsNoTracking()
            .ToListAsync(ct);

        var activas = carrerasEnCurso.Select(c => c.Id).ToHashSet();

        // Limpiar hashes de carreras que ya no están activas
        foreach (var key in _hashPorCarrera.Keys)
        {
            if (!activas.Contains(key))
            {
                _hashPorCarrera.TryRemove(key, out _);
            }
        }

        foreach (var car in carrerasEnCurso)
        {
            // Snapshot mínimo de participaciones de ese evento
            var parts = await db.Participaciones
                .Where(p => p.EventoId == car.EventoId)
                .Select(p => new
                {
                    p.Id,
                    p.Estado,
                    p.Puesto,
                    p.NumeroCorredor,
                    p.Progreso
                })
                .AsNoTracking()
                .ToListAsync(ct);

            var hash = ComputeSnapshotHash(parts);

            if (!_hashPorCarrera.TryGetValue(car.Id, out var anterior) || !string.Equals(anterior, hash, StringComparison.Ordinal))
            {
                _hashPorCarrera[car.Id] = hash;
                _logger.LogDebug("Cambio detectado en carrera {CarreraId}. Notificando...", car.Id);
                await _hub.Clients.Group(car.Id.ToString()).SendAsync("RaceUpdated", car.Id, cancellationToken: ct);
            }
        }
    }

    private static string ComputeSnapshotHash(IEnumerable<dynamic> parts)
    {
        // Construye una cadena determinística con los campos relevantes de cada participación
        var sb = new StringBuilder();
        foreach (var p in parts.OrderBy(x => (Guid)x.Id))
        {
            sb.Append(p.Id).Append('|')
              .Append((int)p.Estado).Append('|')
              .Append((uint)(p.NumeroCorredor ?? 0u)).Append('|')
              .Append((uint)p.Puesto).Append('|');

            // Progreso: ordenar por punto y usar Ticks para estabilidad
            var prog = p.Progreso as IDictionary<uint, TimeSpan> ?? new Dictionary<uint, TimeSpan>();
            foreach (var kv in prog.OrderBy(k => k.Key))
            {
                sb.Append(kv.Key).Append(':').Append(kv.Value.Ticks).Append(',');
            }
            sb.Append(';');
        }

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var hash = Convert.ToHexString(sha.ComputeHash(bytes));
        return hash;
    }
}
