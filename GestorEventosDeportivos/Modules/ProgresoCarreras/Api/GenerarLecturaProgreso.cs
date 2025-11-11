using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using GestorEventosDeportivos.Hubs;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Api;

public static class GenerarLecturaProgreso
{
    public static void MapGenerarLecturaProgresoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/progreso/generar-lectura", GenerarLectura)
            .WithName("GenerarLecturaProgreso");
    }

    private static async Task<IResult> GenerarLectura(
        [FromServices] AppDbContext context,
        [FromServices] IHubContext<RaceHub> hubContext)
    {
        try
        {
            // Obtiene las participaciones donde el evente este en curso y el participante tambien
            var participacionesEnCurso = await context.Participaciones
                .Include(p => p.Evento)
                .Include(p => p.Participante)
                .Where(p => p.Evento!.EstadoEvento == EstadoEvento.EnCurso 
                         && p.Estado == EstadoParticipanteEnCarrera.EnCurso)
                .ToListAsync();

            if (!participacionesEnCurso.Any())
            {
                return Results.NotFound(new
                {
                    mensaje = "No hay participantes en curso en eventos activos",
                    success = false
                });
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
                return Results.NotFound(new
                {
                    mensaje = "No se encontro la carrera asociada",
                    success = false
                });
            }

            // Determinar el siguiente punto de control
            var ultimoPuntoRegistrado = participacionSeleccionada.Progreso.Keys.Any() 
                ? participacionSeleccionada.Progreso.Keys.Max() 
                : 0;

            var siguientePunto = ultimoPuntoRegistrado + 1;
            var totalPuntos = carrera.PuntosDeControl.Count;

            if (siguientePunto > totalPuntos)
            {
                return Results.BadRequest(new
                {
                    mensaje = "El participante ya completo todos los puntos de control",
                    success = false
                });
            }

            // Generar un tiempo simulado para el punto de control
            var tiempoBase = participacionSeleccionada.Progreso.Values.Any() 
                ? participacionSeleccionada.Progreso.Values.Max() 
                : TimeSpan.Zero;

            var tiempoAdicional = TimeSpan.FromMinutes(random.Next(15, 30)); // Entre 15 y 30 minutos
            var nuevoTiempo = tiempoBase.Add(tiempoAdicional);

            // Agregar el nuevo registro de progreso
            participacionSeleccionada.Progreso[siguientePunto] = nuevoTiempo;

            // Si ya completo todos los puntos, cambiar estado a Completada
            if (siguientePunto == totalPuntos)
            {
                participacionSeleccionada.Estado = EstadoParticipanteEnCarrera.Completada;
            }

            await context.SaveChangesAsync();
            if (carrera != null)
        {
            var groupName = $"race-{carrera.Id}";
            await hubContext.Clients.Group(groupName)
                .SendAsync("ProgresoActualizado", participacionSeleccionada.ParticipanteId, (uint)siguientePunto);
        }

            return Results.Ok(new
            {
                mensaje = "Lectura generada exitosamente",
                success = true,
                datos = new
                {
                    participante = new
                    {
                        id = participacionSeleccionada.ParticipanteId,
                        nombre = participacionSeleccionada.Participante?.Nombre,
                        apellido = participacionSeleccionada.Participante?.Apellido,
                        numeroCorredor = participacionSeleccionada.NumeroCorredor
                    },
                    evento = new
                    {
                        id = participacionSeleccionada.EventoId,
                        nombre = participacionSeleccionada.Evento?.Nombre
                    },
                    puntoDeControl = siguientePunto,
                    totalPuntosDeControl = totalPuntos,
                    tiempoRegistrado = nuevoTiempo.ToString(@"hh\:mm\:ss"),
                    estadoParticipacion = participacionSeleccionada.Estado.ToString(),
                    completado = siguientePunto == totalPuntos
                }
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Error al generar la lectura de progreso"
            );
        }
    }
}
