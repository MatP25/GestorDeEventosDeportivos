using GestorEventosDeportivos.Components;
using Microsoft.EntityFrameworkCore;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using GestorEventosDeportivos.Modules.Usuarios.Application.Services;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Application;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Api;
using GestorEventosDeportivos.Modules.Carreras.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<IUsuarioServices, UsuarioServices>();
builder.Services.AddTransient<IProgresoService , ProgresoServices>();
builder.Services.AddTransient<ICarreraService , CarreraService>();

// Generador automaticos de lecturas (solo si esta habilitado)
var generadorHabilitado = builder.Configuration.GetValue<bool>("GeneradorLecturas:Habilitado", true);
if (generadorHabilitado)
{
    builder.Services.AddHostedService<GeneradorAutomaticoLecturas>();
}

//DB Context
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

var app = builder.Build();

// Insertar datos de prueba
InitData.InsertData(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Mapear endpoints de API
app.MapGenerarLecturaProgresoEndpoints();
app.MapControlGeneradorEndpoints();

app.Run();
