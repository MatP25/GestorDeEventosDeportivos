using System.Collections.Concurrent;
using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RaceSimulatorApp;

public class Program
{
    public static async Task Main(string[] args)
    {
    var argsDict = ParseArgs(args);

    // Parametros CLI con defaults (deferimos carreraId hasta despues de --list)
    int durationSec = GetInt("duration", 150);
    double dropoutProb = GetDouble("dropout", 0.04);
    double dqProb = GetDouble("dq", 0.02);
    // Asegurar rango [0,1]
    dropoutProb = Math.Clamp(dropoutProb, 0.0, 1.0);
    dqProb = Math.Clamp(dqProb, 0.0, 1.0);
    int? seed = TryGetInt("seed");
    string? connStrOverride = GetString("connectionString", null);
    // Parametros de BD para construir cadena si se pasan individualmente
    string? dbHost = GetString("db-host", null);
    int? dbPort = TryGetInt("db-port");
    string? dbName = GetString("db-database", null);
    string? dbUser = GetString("db-user", null);
    string? dbPassword = GetString("db-password", null);
    string? dbSslMode = GetString("db-sslmode", "None");
    bool allowPKR = string.Equals(GetString("db-allowPublicKeyRetrieval", "true"), "true", StringComparison.OrdinalIgnoreCase);

// Configuracion: usar connection string del appsettings de la app (o override)
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(Path.Combine("..", "appsettings.json"), optional: true)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

        string? connStrFromCliParts = null;
        if (!string.IsNullOrWhiteSpace(dbHost) && !string.IsNullOrWhiteSpace(dbName) && !string.IsNullOrWhiteSpace(dbUser) && !string.IsNullOrWhiteSpace(dbPassword))
        {
            var portPart = dbPort.HasValue ? dbPort.Value : 3306;
            var sslPart = string.IsNullOrWhiteSpace(dbSslMode) ? "None" : dbSslMode;
            var pkrPart = allowPKR ? "True" : "False";
            connStrFromCliParts = $"Server={dbHost};Port={portPart};Database={dbName};User={dbUser};Password={dbPassword};SslMode={sslPart};AllowPublicKeyRetrieval={pkrPart}";
        }

        string connStr = connStrOverride
            ?? connStrFromCliParts
            ?? config.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=GestorEventosDeportivos;User=appuser;Password=apppassword";

    Console.WriteLine($"Usando DB -> host={(dbHost ?? "(cfg/appsettings)")}, port={(dbPort?.ToString() ?? "(cfg)")}, database={(dbName ?? "(cfg)")}");

        // RNG por corredor (determinista con seed + dorsal, seguro en paralelo)
        Random rng = seed.HasValue ? new Random(seed.Value) : new Random();

// Politica de reintentos para operaciones de BD (exponencial con jitter)
        Random jitter = seed.HasValue ? new Random(seed.Value) : new Random();
        // Crear politica de reintentos (Polly) - tipos concretos sin alias
        var retryPolicy = Policy
            .Handle<DbUpdateException>()
            .Or<DbUpdateConcurrencyException>()
            .Or<MySqlConnector.MySqlException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(5, attempt =>
                TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(jitter.Next(50, 250)));

// Context factory por operacion (nuevo DbContext por evento)
        AppDbContext NewDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connStr, ServerVersion.AutoDetect(connStr))
                .EnableSensitiveDataLogging(false)
                .Options;
            return new AppDbContext(options);
        }

    var start = DateTime.UtcNow;
    TimeSpan Elapsed() => DateTime.UtcNow - start;
    string FormatShort(TimeSpan ts) => ts.ToString(@"mm\:ss");
    string FormatFull(TimeSpan ts) => ts.ToString(@"mm\:ss\.fff");
    string Ts() => $"[{FormatShort(Elapsed())}]";

        // Modo HTTP para probar balanceador: genera N requests contra el LB y muestra X-Upstream
        if (argsDict.ContainsKey("http-mode"))
        {
            string lbUrl = GetString("lb-url", "http://localhost:8080");
            string endpoint = GetString("endpoint", "/api/progreso/generar-lectura")!;
            int requests = GetInt("requests", 50);
            int concurrency = Math.Max(1, GetInt("concurrency", 5));

            Console.WriteLine($"HTTP mode -> url={lbUrl}, endpoint={endpoint}, requests={requests}, concurrency={concurrency}");

            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                KeepAlivePingDelay = TimeSpan.FromSeconds(15),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests
            };
            using var http = new HttpClient(handler) { BaseAddress = new Uri(lbUrl), Timeout = TimeSpan.FromSeconds(10) };

            var sem = new SemaphoreSlim(concurrency);
            var upstreamCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var statusCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var httpTasks = Enumerable.Range(0, requests).Select(async idx =>
            {
                await sem.WaitAsync();
                try
                {
                    var t0 = DateTime.UtcNow;
                    using var resp = await http.GetAsync(endpoint);
                    var dt = DateTime.UtcNow - t0;
                    resp.Headers.TryGetValues("X-Upstream", out var ups);
                    resp.Headers.TryGetValues("X-Upstream-Status", out var upst);
                    var up = ups?.FirstOrDefault() ?? "(desconocido)";
                    var st = upst?.FirstOrDefault() ?? ((int)resp.StatusCode).ToString();
                    upstreamCounts.AddOrUpdate(up, 1, (_, v) => v + 1);
                    statusCounts.AddOrUpdate(st, 1, (_, v) => v + 1);
                    Console.WriteLine($"{Ts()} [{idx+1}/{requests}] {resp.StatusCode} via {up} ({dt.TotalMilliseconds:F0} ms)");
                }
                catch (Exception ex)
                {
                    upstreamCounts.AddOrUpdate("(error)", 1, (_, v) => v + 1);
                    Console.WriteLine($"{Ts()} [{idx+1}/{requests}] ERROR: {ex.Message}");
                }
                finally
                {
                    sem.Release();
                }
            }).ToArray();

            await Task.WhenAll(httpTasks);

            Console.WriteLine("Resumen por upstream:");
            foreach (var kv in upstreamCounts.OrderByDescending(k => k.Value))
                Console.WriteLine($" - {kv.Key}: {kv.Value}");
            Console.WriteLine("Resumen por status:");
            foreach (var kv in statusCounts.OrderByDescending(k => k.Value))
                Console.WriteLine($" - {kv.Key}: {kv.Value}");
            return;
        }

        // Simulacion completa via HTTP (LB): usa endpoints explicitos para registrar lecturas y estados
        if (argsDict.ContainsKey("http-sim"))
        {
            string lbUrl = GetString("lb-url", "http://localhost:8080");
            Console.WriteLine($"HTTP SIM -> url={lbUrl}");

            var http = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                KeepAlivePingDelay = TimeSpan.FromSeconds(15),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests
            }) { BaseAddress = new Uri(lbUrl), Timeout = TimeSpan.FromSeconds(10) };

            Guid carreraIdForHttp = GetGuid("carrera", required: true);

            // Cargar contexto base para conocer puntos y participantes
            Carrera carreraHttp = default!;
            List<PuntoDeControl> puntosHttp = new();
            List<Participacion> participacionesHttp = new();
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using var db = NewDb();
                carreraHttp = await db.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carreraIdForHttp)
                    ?? throw new Exception($"Carrera {carreraIdForHttp} no encontrada");
                puntosHttp = await db.PuntosDeControl.Where(p => p.CarreraId == carreraHttp.Id).OrderBy(p => p.Posicion).AsNoTracking().ToListAsync();
                participacionesHttp = await db.Participaciones
                    .Include(p => p.Participante)
                    .Where(p => p.EventoId == carreraHttp.EventoId)
                    .AsNoTracking()
                    .ToListAsync();
            });

            int checkpointsHttp = puntosHttp.Count;
            var corredoresHttp = participacionesHttp.Where(p => p.NumeroCorredor.HasValue).OrderBy(p => p.NumeroCorredor!.Value).ToList();

            // Seleccion exacta DNF/DQ como antes
            int runnerCountH = corredoresHttp.Count;
            int targetAbandonsH = (int)Math.Floor(dropoutProb * runnerCountH);
            int targetDQH = (int)Math.Floor(dqProb * runnerCountH);
            var samplingRngH = seed.HasValue ? new Random(HashCode.Combine(seed.Value, 137, 911)) : new Random();
            var shuffledH = corredoresHttp.OrderBy(_ => samplingRngH.Next()).ToList();
            var dqSetH = new HashSet<Guid>(shuffledH.Take(targetDQH).Select(c => c.ParticipanteId));
            var dnfSetH = new HashSet<Guid>(shuffledH.Skip(targetDQH).Take(targetAbandonsH).Select(c => c.ParticipanteId));
            var dropCpMapH = new Dictionary<Guid, uint>();
            foreach (var c in corredoresHttp)
            {
                if (!dqSetH.Contains(c.ParticipanteId) && !dnfSetH.Contains(c.ParticipanteId)) continue;
                if (checkpointsHttp >= 3) dropCpMapH[c.ParticipanteId] = (uint)samplingRngH.Next(2, checkpointsHttp);
                else if (checkpointsHttp == 2) dropCpMapH[c.ParticipanteId] = 2; else dropCpMapH[c.ParticipanteId] = 1;
            }

            TimeSpan[] baseSegmentsHttp = Enumerable.Repeat(TimeSpan.FromSeconds((double)durationSec / Math.Max(1, checkpointsHttp)), Math.Max(1, checkpointsHttp)).ToArray();

            async Task<(bool ok, string upstream)> PostJsonAsync(string path, object payload)
            {
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var resp = await http.PostAsync(path, content);
                resp.Headers.TryGetValues("X-Upstream", out var ups);
                var up = ups?.FirstOrDefault() ?? "(desconocido)";
                return (resp.IsSuccessStatusCode, up);
            }

            Console.WriteLine($"{Ts()} HTTP SIM Race {carreraHttp.Id} started. Checkpoints={checkpointsHttp}. Runners={corredoresHttp.Count}.");

            int finishedCount = 0, abandonedCount = 0, dqCount = 0;
            var upstreamCountsSim = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var tasksHttp = corredoresHttp.Select(corredor => Task.Run(async () =>
            {
                var dorsal = corredor.NumeroCorredor ?? 0;
                var prng = seed.HasValue ? new Random(HashCode.Combine(seed.Value, (int)dorsal, 7919)) : new Random(Guid.NewGuid().GetHashCode());
                double NextGaussianLoc(Random r, double mean, double std)
                {
                    double u1 = 1 - r.NextDouble();
                    double u2 = 1 - r.NextDouble();
                    double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                    return mean + std * z;
                }
                double speedFactor = Math.Clamp(NextGaussianLoc(prng, 1.0, 0.10), 0.7, 1.4);
                TimeSpan elapsed = TimeSpan.Zero;

                bool plannedDQ = dqSetH.Contains(corredor.ParticipanteId);
                bool plannedDNF = !plannedDQ && dnfSetH.Contains(corredor.ParticipanteId);
                uint dropAt = (plannedDQ || plannedDNF) && dropCpMapH.TryGetValue(corredor.ParticipanteId, out var v) ? v : uint.MaxValue;

                for (uint i = 1; i <= (uint)checkpointsHttp; i++)
                {
                    if (i == dropAt && (plannedDQ || plannedDNF))
                    {
                        var res = await PostJsonAsync(plannedDQ ? "/api/progreso/descalificar" : "/api/progreso/abandonar",
                            new { carreraId = carreraHttp.Id, participanteId = corredor.ParticipanteId });
                        upstreamCountsSim.AddOrUpdate(res.upstream, 1, (_, v) => v + 1);
                        Console.WriteLine($"{Ts()} Runner {dorsal} {(plannedDQ ? "descalificado" : "abandonó")} via {res.upstream} => {(res.ok ? "OK" : "FAIL")}");
                        if (res.ok)
                        {
                            if (plannedDQ) Interlocked.Increment(ref dqCount); else Interlocked.Increment(ref abandonedCount);
                        }
                        return;
                    }

                    double seg = baseSegmentsHttp[i - 1].TotalSeconds * speedFactor * Math.Clamp(NextGaussianLoc(prng, 1.0, 0.05), 0.85, 1.20);
                    seg = Math.Max(0.3, seg);
                    var delay = TimeSpan.FromSeconds(seg);
                    await Task.Delay(delay);
                    elapsed += delay;

                    var resPass = await PostJsonAsync("/api/progreso/registrar", new { carreraId = carreraHttp.Id, participanteId = corredor.ParticipanteId, puntoDeControlPosicion = i, elapsedMs = (long)elapsed.TotalMilliseconds });
                    upstreamCountsSim.AddOrUpdate(resPass.upstream, 1, (_, v) => v + 1);
                    Console.WriteLine($"{Ts()} Runner {dorsal} CP{i} via {resPass.upstream} => {(resPass.ok ? "OK" : "FAIL")}");
                }

                Interlocked.Increment(ref finishedCount);
                Console.WriteLine($"{Ts()} Runner {dorsal} FIN via LB");
            })).ToArray();

            var timeoutHttp = Task.Delay(TimeSpan.FromSeconds(durationSec * 1.2 + 10));
            await Task.WhenAny(Task.WhenAll(tasksHttp), timeoutHttp);
            Console.WriteLine($"{Ts()} HTTP SIM Race {carreraHttp.Id} done.");
            Console.WriteLine($"Summary: Finalizados={finishedCount}, Abandonos={abandonedCount}, Descalificados={dqCount}.");
            if (upstreamCountsSim.Count > 0)
            {
                Console.WriteLine("Upstream summary:");
                foreach (var kv in upstreamCountsSim.OrderByDescending(k => k.Value))
                    Console.WriteLine($" - {kv.Key}: {kv.Value}");
            }
            return;
        }

        // Listado de carreras (--list) antes de requerir --carrera
        if (argsDict.ContainsKey("list"))
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using var db = NewDb();
                var q = db.Carreras
                    .Include(c => c.Evento)
                    .Include(c => c.PuntosDeControl)
                    .AsNoTracking()
                    .OrderBy(c => c.Evento!.FechaInicio);

                // Por defecto mostrar solo carreras con Evento en SinComenzar
                if (!argsDict.ContainsKey("list-all"))
                {
                    q = q.Where(c => c.Evento != null && c.Evento!.EstadoEvento == EstadoEvento.SinComenzar)
                         .OrderBy(c => c.Evento!.FechaInicio);
                }

                var list = await q.ToListAsync();
                Console.WriteLine(argsDict.ContainsKey("list-all") ? "Carreras (todas):" : "Carreras SinComenzar:");
                foreach (var c in list)
                {
                    var partCount = await db.Participaciones.CountAsync(p => p.EventoId == c.EventoId && p.NumeroCorredor != null);
                    Console.WriteLine($" - {c.Evento!.Nombre} (CarreraId={c.Id}) Estado={c.Evento!.EstadoEvento} PCs={c.PuntosDeControl.Count} RunnersConDorsal={partCount}");
                }
            });
            return;
        }

        // Reset de una carrera a SinComenzar: --reset <carreraId>
        if (argsDict.ContainsKey("reset"))
        {
            var resetId = GetGuid("reset", required: true);
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using var db = NewDb();
                var car = await db.Carreras.Include(c => c.Evento).FirstOrDefaultAsync(c => c.Id == resetId)
                          ?? throw new Exception($"Carrera {resetId} no encontrada");
                var parts = await db.Participaciones.Where(p => p.EventoId == car.EventoId).ToListAsync();
                foreach (var p in parts)
                {
                    p.Estado = EstadoParticipanteEnCarrera.SinComenzar;
                    p.Puesto = 0;
                    p.Progreso.Clear();
                }
                if (car.Evento != null)
                {
                    car.Evento.EstadoEvento = EstadoEvento.SinComenzar;
                }
                await db.SaveChangesAsync();
                Console.WriteLine($"Carrera {resetId} reseteada a SinComenzar. Participaciones reiniciadas.");
            });
            return;
        }

    Guid carreraId = GetGuid("carrera", required: true);

// Cargar carrera + evento + puntos + participaciones
Carrera carrera = default!;
Evento evento = default!;
List<PuntoDeControl> puntos = new();
List<Participacion> participaciones = new();

        await retryPolicy.ExecuteAsync(async () =>
        {
            await using var db = NewDb();
            carrera = await db.Carreras.AsNoTracking().FirstOrDefaultAsync(c => c.Id == carreraId)
                ?? throw new Exception($"Carrera {carreraId} no encontrada");
            evento = await db.Eventos.FirstAsync(e => e.Id == carrera.EventoId);
            puntos = await db.PuntosDeControl.Where(p => p.CarreraId == carrera.Id).OrderBy(p => p.Posicion).AsNoTracking().ToListAsync();
            participaciones = await db.Participaciones
                .Include(p => p.Participante)
                .Where(p => p.EventoId == carrera.EventoId)
                .AsNoTracking()
                .ToListAsync();
        });

int checkpoints = puntos.Count;
if (checkpoints == 0)
{
    Console.WriteLine($"{Ts()} La carrera no tiene puntos de control; agregue al menos 1.");
    return;
}

// Filtrar corredores: solo con numero de corredor asignado (pagos confirmados)
var corredores = participaciones
    .Where(p => p.NumeroCorredor.HasValue)
    .OrderBy(p => p.NumeroCorredor!.Value)
    .ToList();

    // Seleccion exacta por porcentajes (sin reemplazo)
    int runnerCount = corredores.Count;
    int targetAbandons = (int)Math.Floor(dropoutProb * runnerCount);
    int targetDQ = (int)Math.Floor(dqProb * runnerCount);

    // Muestreo determinista si hay seed, si no aleatorio
    var samplingRng = seed.HasValue ? new Random(HashCode.Combine(seed.Value, 137, 911)) : new Random();
    var shuffled = corredores.OrderBy(_ => samplingRng.Next()).ToList();
    var dqSet = new HashSet<Guid>(shuffled.Take(targetDQ).Select(c => c.ParticipanteId));
    var remaining = shuffled.Skip(targetDQ).ToList();
    var dnfSet = new HashSet<Guid>(remaining.Take(targetAbandons).Select(c => c.ParticipanteId));

    // Asignar checkpoint de caida para cada seleccionado (>=2 si hay al menos 3 PCs)
    var dropCpMap = new Dictionary<Guid, uint>();
    foreach (var c in corredores)
    {
        if (!dqSet.Contains(c.ParticipanteId) && !dnfSet.Contains(c.ParticipanteId)) continue;
        if (checkpoints >= 3)
        {
            dropCpMap[c.ParticipanteId] = (uint)samplingRng.Next(2, checkpoints); // 2..(checkpoints-1)
        }
        else if (checkpoints == 2)
        {
            dropCpMap[c.ParticipanteId] = 2; // antes del registro del ultimo punto
        }
        else // checkpoints == 1
        {
            dropCpMap[c.ParticipanteId] = 1;
        }
    }

    Console.WriteLine($"{Ts()} Race {carrera.Id} started. Duration {durationSec}s. Checkpoints={checkpoints}. Runners={corredores.Count}.");

// Poner todos los corredores EnCurso al inicio y marcar evento EnCurso
        await retryPolicy.ExecuteAsync(async () =>
        {
            await using var db = NewDb();
            var ev = await db.Eventos.FirstAsync(e => e.Id == carrera.EventoId);
            ev.EstadoEvento = EstadoEvento.EnCurso;

            var parts = await db.Participaciones.Where(p => p.EventoId == carrera.EventoId).ToListAsync();
            foreach (var p in parts)
            {
                if (p.Estado == EstadoParticipanteEnCarrera.SinComenzar)
                    p.Estado = EstadoParticipanteEnCarrera.EnCurso;
            }
            await db.SaveChangesAsync();
        });

// Generadores de tiempo: asignar factor de velocidad por corredor y segmentación para que el total ~ durationSec
        // Distribucion normal con Random provisto (evita Random compartido entre tareas)
        double NextGaussian(Random r, double mean, double stddev)
        {
            // Box–Muller
            double u1 = 1.0 - r.NextDouble();
            double u2 = 1.0 - r.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stddev * randStdNormal;
        }

// Estadisticas para resumen
var cpTimes = new ConcurrentDictionary<uint, ConcurrentBag<TimeSpan>>();
for (uint i = 1; i <= (uint)checkpoints; i++) cpTimes[i] = new ConcurrentBag<TimeSpan>();
int finished = 0, abandoned = 0, disqualified = 0;
int positionCounter = 0; // para asignar Puesto en orden real de llegada

TimeSpan[] baseSegments = Enumerable.Repeat(TimeSpan.FromSeconds((double)durationSec / checkpoints), checkpoints).ToArray();

// Simulacion por corredor
    var tasks = corredores.Select(corredor => Task.Run(async () =>
    {
    // factor de velocidad por corredor (media 1.0, 10% desv.)
        var dorsal = corredor.NumeroCorredor ?? 0;
        var prng = seed.HasValue
            ? new Random(HashCode.Combine(seed.Value, (int)dorsal, 7919))
            : new Random(Guid.NewGuid().GetHashCode());

        double speedFactor = Math.Clamp(NextGaussian(prng, 1.0, 0.10), 0.7, 1.4);
    TimeSpan elapsed = TimeSpan.Zero;

    // Destino preseleccionado: Finish / Abandono / DQ, con checkpoint de caida definido
    bool plannedDQ = dqSet.Contains(corredor.ParticipanteId);
    bool plannedDNF = !plannedDQ && dnfSet.Contains(corredor.ParticipanteId);
    uint dropAtCp = (plannedDQ || plannedDNF) && dropCpMap.TryGetValue(corredor.ParticipanteId, out var dcp) ? dcp : uint.MaxValue;

    for (uint i = 1; i <= (uint)checkpoints; i++)
    {
        // Si toca caer aqui, aplicar DNF/DQ (ya preasignado y sin superar cupos)
        if (i == dropAtCp && (plannedDQ || plannedDNF))
        {
            if (plannedDQ)
            {
                var changed = false;
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await using var db = NewDb();
                    var p = await db.Participaciones.FirstAsync(p => p.EventoId == corredor.EventoId && p.ParticipanteId == corredor.ParticipanteId);
                    if (p.Estado == EstadoParticipanteEnCarrera.EnCurso)
                    {
                        p.Estado = EstadoParticipanteEnCarrera.Descalificado;
                        await db.SaveChangesAsync();
                        Interlocked.Increment(ref disqualified);
                        Console.WriteLine($"{Ts()} Runner {dorsal} descalificado (motivo: infracción simulada).");
                        changed = true;
                    }
                });
                if (changed) return;
            }
            else if (plannedDNF)
            {
                var changed = false;
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await using var db = NewDb();
                    var p = await db.Participaciones.FirstAsync(p => p.EventoId == corredor.EventoId && p.ParticipanteId == corredor.ParticipanteId);
                    if (p.Estado == EstadoParticipanteEnCarrera.EnCurso)
                    {
                        p.Estado = EstadoParticipanteEnCarrera.Abandonada;
                        await db.SaveChangesAsync();
                        Interlocked.Increment(ref abandoned);
                        Console.WriteLine($"{Ts()} Runner {dorsal} abandonó (motivo: lesión simulada).");
                        changed = true;
                    }
                });
                if (changed) return;
            }
            // Si no se pudo por cap alcanzado, seguimos y el corredor finaliza
        }

        // Tiempo de tramo con ruido (5% por tramo)
        double seg = baseSegments[i - 1].TotalSeconds * speedFactor * Math.Clamp(NextGaussian(prng, 1.0, 0.05), 0.85, 1.20);
        seg = Math.Max(0.3, seg);
        var delay = TimeSpan.FromSeconds(seg);

        await Task.Delay(delay);
        elapsed += delay;

        // Registrar paso por checkpoint i (idempotente: upsert del diccionario)
        var cpPos = i;
        var elapsedCopy = elapsed;
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await using var db = NewDb();
                    var p = await db.Participaciones.FirstAsync(p => p.EventoId == corredor.EventoId && p.ParticipanteId == corredor.ParticipanteId);

                    // Si ya no esta EnCurso, no registrar mas
                    if (p.Estado != EstadoParticipanteEnCarrera.EnCurso && p.Estado != EstadoParticipanteEnCarrera.SinComenzar)
                        return;

                    // idempotencia minima: solo setear si no existe o si el nuevo tiempo es menor
                    if (!p.Progreso.ContainsKey(cpPos) || p.Progreso[cpPos] > elapsedCopy)
                    {
                        p.Progreso[cpPos] = elapsedCopy;
                    }

                    // Primer registro puede mover a EnCurso explicitamente
                    if (p.Estado == EstadoParticipanteEnCarrera.SinComenzar)
                        p.Estado = EstadoParticipanteEnCarrera.EnCurso;

                    // Si es el ultimo punto, marcar completado y asignar puesto
                    if (cpPos == (uint)checkpoints)
                    {
                        p.Estado = EstadoParticipanteEnCarrera.Completada;
                        int pos = Interlocked.Increment(ref positionCounter);
                        p.Puesto = (uint)pos;
                    }

                    await db.SaveChangesAsync();
                });

        cpTimes[cpPos].Add(elapsed);

        if (i == (uint)checkpoints)
        {
                Interlocked.Increment(ref finished);
                        Console.WriteLine($"{Ts()} Runner {dorsal} llegó a meta. Position={positionCounter}. FinishTime={FormatFull(elapsed)}");
        }
        else
        {
                Console.WriteLine($"{Ts()} Runner {dorsal} passed checkpoint {i} (elapsed {FormatFull(elapsed)}).");
        }
    }
    })).ToArray();

// Esperar a que todos terminen o hasta durationSec + 10%
    var timeout = Task.Delay(TimeSpan.FromSeconds(durationSec * 1.1 + 5));
    await Task.WhenAny(Task.WhenAll(tasks), timeout);

// Si todos los corredores activos llegaron, finalizar evento
        await retryPolicy.ExecuteAsync(async () =>
        {
            await using var db = NewDb();
            var estados = await db.Participaciones.Where(p => p.EventoId == carrera.EventoId).Select(p => p.Estado).ToListAsync();
            bool anyRunning = estados.Any(s => s == EstadoParticipanteEnCarrera.EnCurso || s == EstadoParticipanteEnCarrera.SinComenzar);
            if (!anyRunning)
            {
                var ev = await db.Eventos.FirstAsync(e => e.Id == carrera.EventoId);
                ev.EstadoEvento = EstadoEvento.Finalizado;
                await db.SaveChangesAsync();
                Console.WriteLine($"{Ts()} Race {carrera.Id} finalizada (todos los corredores activos llegaron).");
            }
        });

// Resumen final
    int totalRunners = corredores.Count;
    int totalFinished = finished;
    int totalAbandoned = abandoned;
    int totalDQ = disqualified;

        TimeSpan Median(List<TimeSpan> list)
        {
            if (list.Count == 0) return TimeSpan.Zero;
            var ordered = list.OrderBy(t => t).ToList();
            int mid = ordered.Count / 2;
            return ordered.Count % 2 == 1
                ? ordered[mid]
                : TimeSpan.FromTicks((ordered[mid - 1].Ticks + ordered[mid].Ticks) / 2);
        }

        var medians = new Dictionary<uint, TimeSpan>();
        for (uint i = 1; i <= (uint)checkpoints; i++)
        {
            medians[i] = Median(cpTimes[i].ToList());
        }

        Console.WriteLine($"Summary: Finalizados={totalFinished}, Abandonos={totalAbandoned}, Descalificados={totalDQ}.");
        foreach (var kv in medians.OrderBy(k => k.Key))
        {
            Console.WriteLine($" - Mediana CP{kv.Key} = {FormatFull(kv.Value)}");
        }

// Helpers de CLI
        string? GetString(string name, string? def)
            => argsDict.TryGetValue(name, out var v) ? v : def;

        int GetInt(string name, int def)
            => int.TryParse(GetString(name, null), out var v) ? v : def;

        int? TryGetInt(string name)
            => int.TryParse(GetString(name, null), out var v) ? v : null;

        double GetDouble(string name, double def)
        {
            var s = GetString(name, null);
            if (string.IsNullOrWhiteSpace(s)) return def;
            if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v1))
                return v1;
            if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out var v2))
                return v2;
            return def;
        }

        Guid GetGuid(string name, bool required)
        {
            var s = GetString(name, null);
            if (Guid.TryParse(s, out var g)) return g;
            if (required) throw new ArgumentException($"Parámetro --{name} requerido");
            return Guid.Empty;
        }

        Dictionary<string, string> ParseArgs(string[] a)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string? key = null;
            foreach (var token in a)
            {
                if (token.StartsWith("--"))
                {
                    key = token.TrimStart('-');
                    dict[key] = string.Empty;
                }
                else if (key != null)
                {
                    dict[key] = token;
                    key = null;
                }
            }
            return dict;
        }
    }
}
