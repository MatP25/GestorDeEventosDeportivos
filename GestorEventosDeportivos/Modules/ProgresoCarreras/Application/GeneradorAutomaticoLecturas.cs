using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Application;

public class GeneradorAutomaticoLecturas : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GeneradorAutomaticoLecturas> _logger;
    private readonly int _intervaloSegundos;

    public GeneradorAutomaticoLecturas(
        IServiceProvider serviceProvider, 
        ILogger<GeneradorAutomaticoLecturas> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _intervaloSegundos = configuration.GetValue<int>("GeneradorLecturas:IntervaloSegundos", 10);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Generador Automatico de Lecturas iniciado. Intervalo: {Intervalo} segundos", _intervaloSegundos);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerarLectura();
                await Task.Delay(TimeSpan.FromSeconds(_intervaloSegundos), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar lectura automatica");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task GenerarLectura()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Obtener participaciones donde el evento este en curso y el participante tambien
        var participacionesEnCurso = await context.Participaciones
            .Include(p => p.Evento)
            .Include(p => p.Participante)
            .Where(p => p.Evento!.EstadoEvento == EstadoEvento.EnCurso 
                     && p.Estado == EstadoParticipanteEnCarrera.EnCurso)
            .ToListAsync();

        if (!participacionesEnCurso.Any())
        {
            _logger.LogInformation("No hay participantes en curso para generar lecturas");
            return;
        }

        // Seleccionar un participante aleatorio
        var random = new Random();
        var participacionSeleccionada = participacionesEnCurso[random.Next(participacionesEnCurso.Count)];

        // Obtener la carrera para saber los puntos de control
        var carrera = await context.Carreras
            .Include(c => c.PuntosDeControl)
            .FirstOrDefaultAsync(c => c.EventoId == participacionSeleccionada.EventoId);

        if (carrera == null)
        {
            _logger.LogWarning("No se encontró la carrera asociada al evento {EventoId}", participacionSeleccionada.EventoId);
            return;
        }

        // Determinar el siguiente punto de control
        var ultimoPuntoRegistrado = participacionSeleccionada.Progreso.Keys.Any() 
            ? participacionSeleccionada.Progreso.Keys.Max() 
            : 0;

        var siguientePunto = ultimoPuntoRegistrado + 1;
        var totalPuntos = carrera.PuntosDeControl.Count;

        if (siguientePunto > totalPuntos)
        {
            _logger.LogInformation("El participante {NumeroCorredor} ya completó todos los puntos de control", 
                participacionSeleccionada.NumeroCorredor);
            return;
        }

        // Generar un tiempo simulado para el punto de control
        var tiempoBase = participacionSeleccionada.Progreso.Values.Any() 
            ? participacionSeleccionada.Progreso.Values.Max() 
            : TimeSpan.Zero;

        var tiempoAdicional = TimeSpan.FromMinutes(random.Next(15, 30)); // Entre 15 y 30 minutos
        var nuevoTiempo = tiempoBase.Add(tiempoAdicional);

        // Se agregar el nuevo registro de progreso
        participacionSeleccionada.Progreso[siguientePunto] = nuevoTiempo;

        if (siguientePunto == totalPuntos)
        {
            participacionSeleccionada.Estado = EstadoParticipanteEnCarrera.Completada;
        }

        await context.SaveChangesAsync();

        _logger.LogInformation(
            "Lectura generada - Participante: {Nombre} {Apellido} (#{NumeroCorredor}) | Punto: {Punto}/{Total} | Tiempo: {Tiempo} | Estado: {Estado}",
            participacionSeleccionada.Participante?.Nombre,
            participacionSeleccionada.Participante?.Apellido,
            participacionSeleccionada.NumeroCorredor,
            siguientePunto,
            totalPuntos,
            nuevoTiempo.ToString(@"hh\:mm\:ss"),
            participacionSeleccionada.Estado);
    }
}
