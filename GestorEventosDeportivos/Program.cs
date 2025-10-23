using GestorEventosDeportivos.Components;
using Microsoft.EntityFrameworkCore;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using GestorEventosDeportivos.Modules.Usuarios.Application.Services;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Application;
using CurrieTechnologies.Razor.SweetAlert2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<IUsuarioServices, UsuarioServices>();
builder.Services.AddTransient<IProgresoService , ProgresoServices>();

//DB Context
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

//SweetAlert2
builder.Services.AddSweetAlert2(options => {
   options.Theme = SweetAlertTheme.Dark;
 });

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

app.Run();
