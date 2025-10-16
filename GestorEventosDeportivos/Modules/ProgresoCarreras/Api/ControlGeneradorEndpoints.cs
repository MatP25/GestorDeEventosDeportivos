using Microsoft.AspNetCore.Mvc;

namespace GestorEventosDeportivos.Modules.ProgresoCarreras.Api;

public static class ControlGeneradorEndpoints
{
    public static void MapControlGeneradorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/progreso/generador/estado", ObtenerEstado)
            .WithName("ObtenerEstadoGenerador")
            .WithDescription("Obtiene el estado del generador automático de lecturas");

        app.MapPost("/api/progreso/generador/iniciar", IniciarGenerador)
            .WithName("IniciarGenerador")
            .WithDescription("Inicia el generador automático de lecturas");

        app.MapPost("/api/progreso/generador/detener", DetenerGenerador)
            .WithName("DetenerGenerador")
            .WithDescription("Detiene el generador automático de lecturas");
    }

    private static IResult ObtenerEstado([FromServices] IConfiguration configuration)
    {
        var habilitado = configuration.GetValue<bool>("GeneradorLecturas:Habilitado", true);
        var intervalo = configuration.GetValue<int>("GeneradorLecturas:IntervaloSegundos", 10);

        return Results.Ok(new
        {
            habilitado,
            intervaloSegundos = intervalo,
            mensaje = habilitado 
                ? $"El generador está activo y genera lecturas cada {intervalo} segundos" 
                : "El generador está deshabilitado"
        });
    }

    private static IResult IniciarGenerador([FromServices] IConfiguration configuration)
    {
        return Results.Ok(new
        {
            mensaje = "Para habilitar el generador, configure 'GeneradorLecturas:Habilitado' en true en appsettings.json y reinicie la aplicación",
            configuracionActual = new
            {
                habilitado = configuration.GetValue<bool>("GeneradorLecturas:Habilitado", true),
                intervaloSegundos = configuration.GetValue<int>("GeneradorLecturas:IntervaloSegundos", 10)
            }
        });
    }

    private static IResult DetenerGenerador([FromServices] IConfiguration configuration)
    {
        return Results.Ok(new
        {
            mensaje = "Para deshabilitar el generador, configure 'GeneradorLecturas:Habilitado' en false en appsettings.json y reinicie la aplicación",
            configuracionActual = new
            {
                habilitado = configuration.GetValue<bool>("GeneradorLecturas:Habilitado", true),
                intervaloSegundos = configuration.GetValue<int>("GeneradorLecturas:IntervaloSegundos", 10)
            }
        });
    }
}
