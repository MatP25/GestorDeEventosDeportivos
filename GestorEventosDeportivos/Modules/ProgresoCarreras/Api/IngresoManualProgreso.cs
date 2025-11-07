using GestorEventosDeportivos.Modules.ProgresoCarreras.Application;
using Microsoft.AspNetCore.Mvc;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Api;

public static class IngresoManualProgreso
{
    public static void MapIngresoManualProgresoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/progreso/registrar", RegistrarLectura)
            .WithName("RegistrarLecturaProgreso")
            .WithDescription("Registra una lectura de punto de control explícita");

        app.MapPost("/api/progreso/abandonar", Abandonar)
            .WithName("AbandonarParticipante")
            .WithDescription("Marca participante como Abandonado");

        app.MapPost("/api/progreso/descalificar", Descalificar)
            .WithName("DescalificarParticipante")
            .WithDescription("Marca participante como Descalificado");
    }

    public record RegistrarLecturaRequest(Guid carreraId, Guid participanteId, uint puntoDeControlPosicion, long elapsedMs);
    public record EstadoRequest(Guid carreraId, Guid participanteId);

    private static async Task<IResult> RegistrarLectura([FromBody] RegistrarLecturaRequest req, [FromServices] IProgresoService svc)
    {
        await svc.IngresarLecturaPuntoDeControl(req.carreraId, req.participanteId, req.puntoDeControlPosicion, TimeSpan.FromMilliseconds(req.elapsedMs));
        return Results.Ok(new { ok = true });
    }

    private static async Task<IResult> Abandonar([FromBody] EstadoRequest req, [FromServices] IProgresoService svc)
    {
        await svc.AbandonarCarrera(req.carreraId, req.participanteId);
        return Results.Ok(new { ok = true });
    }

    private static async Task<IResult> Descalificar([FromBody] EstadoRequest req, [FromServices] IProgresoService svc)
    {
        if (svc is ProgresoServices impl)
        {
            await impl.DescalificarParticipante(req.carreraId, req.participanteId);
            return Results.Ok(new { ok = true });
        }
        await Task.CompletedTask;
        return Results.BadRequest(new { ok = false, error = "No se pudo descalificar" });
    }
}
