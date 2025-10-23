using GestorEventosDeportivos.Components;
using Microsoft.EntityFrameworkCore;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using GestorEventosDeportivos.Modules.Usuarios.Application.Services;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Application;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Api;
using GestorEventosDeportivos.Modules.Carreras.Application.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<IUsuarioServices, UsuarioServices>();
builder.Services.AddTransient<IProgresoService , ProgresoServices>();
builder.Services.AddTransient<ICarreraService , CarreraService>();
// Removed in-memory admin demo service; administration now uses real services and the DB.

// HttpContext y HttpClient
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Auth (servicios)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout"; // no endpoint directo, redirige a POST /auth/logout
        options.AccessDeniedPath = "/acceso-denegado";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Mapear endpoints de API
app.MapGenerarLecturaProgresoEndpoints();
app.MapControlGeneradorEndpoints();

// Endpoints de autenticación (cookies)
app.MapPost("/auth/login", async (HttpContext http, AppDbContext db) =>
{
    var form = await http.Request.ReadFormAsync();
    var email = form["Email"].ToString();
    var password = form["Password"].ToString();

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect($"/login?error=Datos%20inv%C3%A1lidos");
    }

    // NOTA: Password en texto plano solo para pruebas. Considerar hashing en producción.
    var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    if (usuario is null)
    {
        return Results.Redirect($"/login?error=Credenciales%20inv%C3%A1lidas");
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
        new Claim(ClaimTypes.Email, usuario.Email)
    };

    // Rol según el tipo
    var role = usuario is Administrador ? "Admin" : "Participante";
    claims.Add(new Claim(ClaimTypes.Role, role));

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(claimsIdentity);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    // Siempre redirigir al Home
    return Results.Redirect("/");
});

app.MapPost("/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
}).RequireAuthorization();

// Nota: si se desea soportar ReturnUrl, reintroducir un validador de URLs locales para evitar open-redirects.

app.Run();
