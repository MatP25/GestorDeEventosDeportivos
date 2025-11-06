using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

public class InitData
{
    public static void InsertData(IApplicationBuilder applicationBuilder)
    {

        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            AppDbContext? context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            if (context is null)
            {
                throw new InvalidOperationException("No se pudo obtener el contexto de la base de datos.");
            }

            EntityEntry<Evento> evento1, evento2, evento3, evento4, evento5;
            EntityEntry<Carrera> carrera1, carrera2, carrera3, carrera4, carrera5;
            EntityEntry<Participante> 
                participante1, participante2, participante3,
                participante4, participante5, participante6,
                participante7, participante8, participante9,
                participante10, participante11, participante12;
            EntityEntry<Participacion> participacion1carrera1, participacion2carrera1, participacion3carrera1,
                participacion1carrera2, participacion2carrera2, participacion3carrera2,
                participacion1carrera3, participacion2carrera3, participacion3carrera3,
                participacion1carrera4, participacion2carrera4, participacion3carrera4,
                participacion1carrera5, participacion2carrera5, participacion3carrera5,
                participacion4carrera5, participacion5carrera5, participacion6carrera5,
                participacion7carrera5, participacion8carrera5, participacion9carrera5,
                participacion10carrera5, participacion11carrera5, participacion12carrera5;

            Console.WriteLine("Verificando si es necesario insertar datos de prueba...");

            if (!context.Administradores.Any() &&
                !context.Eventos.Any() &&
                !context.Carreras.Any() &&
                !context.Participantes.Any() &&
                !context.Participaciones.Any())
            {
                Console.WriteLine("Iniciando datos de prueba...");

                context.Administradores.Add(new()
                {
                    Nombre = "Admin",
                    Apellido = "Principal",
                    Email = "admin@email.com",
                    Password = "admin1234",
                    FechaNac = new DateOnly(1990, 1, 1)
                });

                evento1 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 1",
                    FechaInicio = DateTime.Now.AddMonths(-1),
                    Ubicacion = "San Carlos",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.Finalizado,
                    CapacidadParticipantes = 10
                });

                evento2 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 2",
                    FechaInicio = DateTime.Now.AddMonths(1),
                    Ubicacion = "Maldonado",
                    RegistroHabilitado = true,
                    EstadoEvento = EstadoEvento.SinComenzar,
                    CapacidadParticipantes = 10
                });

                evento3 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 3",
                    FechaInicio = DateTime.Now.AddDays(-5),
                    Ubicacion = "Punta del Este",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.Finalizado,
                    CapacidadParticipantes = 10
                });

                evento4 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón en Curso",
                    FechaInicio = DateTime.Now,
                    Ubicacion = "Montevideo",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.EnCurso,
                    CapacidadParticipantes = 10
                });

                evento5 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 5",
                    FechaInicio = DateTime.Now.AddMonths(2),
                    Ubicacion = "Colonia",
                    RegistroHabilitado = true,
                    EstadoEvento = EstadoEvento.SinComenzar,
                    CapacidadParticipantes = 20
                });

                carrera1 = context.Carreras.Add(new()
                {
                    EventoId = evento1.Entity.Id,
                    Longitud = 42000,
                    Ganador = "Usuario Uno",
                    TiempoGanador = new TimeSpan(3, 15, 30),
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 10" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 21" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Punto 3 - Km 32" },
                        new PuntoDeControl { Posicion = 4, Ubicacion = "Punto 4 - Km 42" }
                    },
                    CantidadParticipacionesPagas = 3
                });

                carrera2 = context.Carreras.Add(new()
                {
                    EventoId = evento2.Entity.Id,
                    Longitud = 21000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 10" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Punto 3 - Km 15" },
                        new PuntoDeControl { Posicion = 4, Ubicacion = "Punto 4 - Km 21" }
                    },
                    CantidadParticipacionesPagas = 3
                });

                carrera3 = context.Carreras.Add(new()
                {
                    EventoId = evento3.Entity.Id,
                    Longitud = 20000,
                    Ganador = "Usuario Tres",
                    TiempoGanador = new TimeSpan(1, 30, 12),
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 10" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 20" }
                    },
                    CantidadParticipacionesPagas = 3
                });

                carrera4 = context.Carreras.Add(new()
                {
                    EventoId = evento4.Entity.Id,
                    Longitud = 10000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 10" }
                    },
                    CantidadParticipacionesPagas = 3
                });

                carrera5 = context.Carreras.Add(new()
                {
                    EventoId = evento5.Entity.Id,
                    Longitud = 15000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 10" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Punto 3 - Km 15" }
                    },
                    CantidadParticipacionesPagas = 8
                });

                // Eventos y carreras adicionales
                var evento6 = context.Eventos.Add(new()
                {
                    Nombre = "Media Maratón 6",
                    FechaInicio = DateTime.Now.AddDays(10),
                    Ubicacion = "Paysandú",
                    RegistroHabilitado = true,
                    EstadoEvento = EstadoEvento.SinComenzar,
                    CapacidadParticipantes = 100
                });

                var evento7 = context.Eventos.Add(new()
                {
                    Nombre = "Cross 7",
                    FechaInicio = DateTime.Now.AddDays(-12),
                    Ubicacion = "Rocha",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.Finalizado,
                    CapacidadParticipantes = 150
                });

                var evento8 = context.Eventos.Add(new()
                {
                    Nombre = "10K 8",
                    FechaInicio = DateTime.Now,
                    Ubicacion = "Florida",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.EnCurso,
                    CapacidadParticipantes = 120
                });

                var evento9 = context.Eventos.Add(new()
                {
                    Nombre = "Trail 9",
                    FechaInicio = DateTime.Now.AddDays(-30),
                    Ubicacion = "Minas",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.Finalizado,
                    CapacidadParticipantes = 200
                });

                var carrera6 = context.Carreras.Add(new()
                {
                    EventoId = evento6.Entity.Id,
                    Longitud = 21000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Km 10" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Km 15" },
                        new PuntoDeControl { Posicion = 4, Ubicacion = "Km 21" }
                    },
                    CantidadParticipacionesPagas = 25
                });

                var carrera7 = context.Carreras.Add(new()
                {
                    EventoId = evento7.Entity.Id,
                    Longitud = 10000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Km 3" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Km 6" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Km 10" }
                    },
                    CantidadParticipacionesPagas = 30
                });

                var carrera8 = context.Carreras.Add(new()
                {
                    EventoId = evento8.Entity.Id,
                    Longitud = 10000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Km 10" }
                    },
                    CantidadParticipacionesPagas = 35
                });

                var carrera9 = context.Carreras.Add(new()
                {
                    EventoId = evento9.Entity.Id,
                    Longitud = 30000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Km 7" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Km 14" },
                        new PuntoDeControl { Posicion = 3, Ubicacion = "Km 21" },
                        new PuntoDeControl { Posicion = 4, Ubicacion = "Km 30" }
                    },
                    CantidadParticipacionesPagas = 40
                });

                participante1 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Uno",
                    Email = "usuario1@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1995, 5, 5)
                });

                participante2 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Dos",
                    Email = "usuario2@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1999, 9, 9)
                });

                participante3 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Tres",
                    Email = "usuario3@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1992, 9, 9)
                });

                participante4 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Cuatro",
                    Email = "usuario4@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1992, 2, 3)
                });
                
                participante5 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Cinco",
                    Email = "usuario5@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1980, 12, 10)
                });

                participante6 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Seis",
                    Email = "usuario6@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1990, 1, 1)
                });
                participante7 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Siete",
                    Email = "usuario7@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1985, 5, 5)
                });

                participante8 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Ocho",
                    Email = "usuario8@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1982, 8, 8)
                });
                participante9 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Nueve",
                    Email = "usuario9@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1995, 5, 5)
                });

                participante10 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Diez",
                    Email = "usuario10@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1990, 10, 10)
                });

                participante11 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Once",
                    Email = "usuario11@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1995, 5, 5)
                });
                participante12 = context.Participantes.Add(new()
                {
                    Nombre = "Usuario",
                    Apellido = "Doce",
                    Email = "usuario12@email.com",
                    Password = "1234",
                    FechaNac = new DateOnly(1998, 12, 12)
                });

                // Crear ~40 participantes adicionales (13..52)
                var participantesExtras = new List<EntityEntry<Participante>>();
                for (int i = 13; i <= 52; i++)
                {
                    var p = context.Participantes.Add(new Participante
                    {
                        Nombre = "Usuario",
                        Apellido = i.ToString(),
                        Email = $"usuario{i}@email.com",
                        Password = "1234",
                        FechaNac = new DateOnly(1990 + (i % 10), (i % 12) + 1, ((i % 27) + 1))
                    });
                    participantesExtras.Add(p);
                }


                participacion1carrera1 = context.Participaciones.Add(
                new Participacion
                {
                    EventoId = carrera1.Entity.EventoId,
                    ParticipanteId = participante1.Entity.Id,
                    NumeroCorredor = 1,
                    Puesto = 1,
                    Estado = EstadoParticipanteEnCarrera.Completada,
                    Progreso = new Dictionary<uint, TimeSpan>
                    {
                        { 1, new TimeSpan(0, 45, 0) },
                        { 2, new TimeSpan(1, 30, 0) },
                        { 3, new TimeSpan(2, 20, 0) },
                        { 4, new TimeSpan(3, 15, 30) }
                    },
                    EstadoPago = EstadoPago.Confirmado
                });

                participacion2carrera1 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera1.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = 2,
                        Puesto = 2,
                        Estado = EstadoParticipanteEnCarrera.Abandonada,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 50, 0) },
                            { 2, new TimeSpan(1, 40, 0) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion3carrera1 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera1.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = 3,
                        Puesto = 3,
                        Estado = EstadoParticipanteEnCarrera.Descalificado,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 25, 0) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion1carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante1.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion2carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion3carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion1carrera3 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera3.Entity.EventoId,
                        ParticipanteId = participante1.Entity.Id,
                        NumeroCorredor = 1,
                        Puesto = 2,
                        Estado = EstadoParticipanteEnCarrera.Completada,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 40, 0) },
                            { 2, new TimeSpan(2, 35, 48) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });
                participacion2carrera3 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera3.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = 2,
                        Puesto = 3,
                        Estado = EstadoParticipanteEnCarrera.Abandonada,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 55, 0) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });
                participacion3carrera3 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera3.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = 3,
                        Puesto = 1,
                        Estado = EstadoParticipanteEnCarrera.Completada,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 30, 0) },
                            { 2, new TimeSpan(1, 30, 12) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });

                // Participaciones para carrera4 (en curso)
                participacion1carrera4 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera4.Entity.EventoId,
                        ParticipanteId = participante1.Entity.Id,
                        NumeroCorredor = 101,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.EnCurso,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 20, 30) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion2carrera4 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera4.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = 102,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.EnCurso,
                        Progreso = new Dictionary<uint, TimeSpan>
                        {
                            { 1, new TimeSpan(0, 22, 15) }
                        },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion3carrera4 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera4.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = 103,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.EnCurso,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                // Participaciones para carrera5 (sin comenzar)
                participacion1carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante1.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });
                
                participacion2carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.EnCurso,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion3carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion4carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante4.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion5carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante5.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion6carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante6.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion7carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante7.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion8carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante8.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.Confirmado
                    });

                participacion9carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante9.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.NoRealizado
                    });

                participacion10carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante10.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.NoRealizado
                    });

                participacion11carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante11.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.NoRealizado
                    });
                    
                participacion12carrera5 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera5.Entity.EventoId,
                        ParticipanteId = participante12.Entity.Id,
                        NumeroCorredor = null,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { },
                        EstadoPago = EstadoPago.NoRealizado
                    });

                participante1.Entity.Carreras.AddRange(new[]
                {
                    participacion1carrera1.Entity,
                    participacion1carrera2.Entity,
                    participacion1carrera3.Entity,
                    participacion1carrera4.Entity,
                    participacion1carrera5.Entity
                });

                participante2.Entity.Carreras.AddRange(new[]
                {
                    participacion2carrera1.Entity,
                    participacion2carrera2.Entity,
                    participacion2carrera3.Entity,
                    participacion2carrera4.Entity,
                    participacion2carrera5.Entity
                });

                participante3.Entity.Carreras.AddRange(new[]
                {
                    participacion3carrera1.Entity,
                    participacion3carrera2.Entity,
                    participacion3carrera3.Entity,
                    participacion3carrera4.Entity,
                    participacion3carrera5.Entity
                });

                participante4.Entity.Carreras.Add(participacion4carrera5.Entity);
                participante5.Entity.Carreras.Add(participacion5carrera5.Entity);
                participante6.Entity.Carreras.Add(participacion6carrera5.Entity);
                participante7.Entity.Carreras.Add(participacion7carrera5.Entity);
                participante8.Entity.Carreras.Add(participacion8carrera5.Entity);
                participante9.Entity.Carreras.Add(participacion9carrera5.Entity);
                participante10.Entity.Carreras.Add(participacion10carrera5.Entity);
                participante11.Entity.Carreras.Add(participacion11carrera5.Entity);
                participante12.Entity.Carreras.Add(participacion12carrera5.Entity);

                carrera1.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera1.Entity,
                    participacion2carrera1.Entity,
                    participacion3carrera1.Entity,
                });
                carrera2.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera2.Entity,
                    participacion2carrera2.Entity,
                    participacion3carrera2.Entity
                });
                carrera3.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera3.Entity,
                    participacion2carrera3.Entity,
                    participacion3carrera3.Entity
                });
                carrera4.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera4.Entity,
                    participacion2carrera4.Entity,
                    participacion3carrera4.Entity
                });
                carrera5.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera5.Entity,
                    participacion2carrera5.Entity,
                    participacion3carrera5.Entity,
                    participacion4carrera5.Entity,
                    participacion5carrera5.Entity,
                    participacion6carrera5.Entity,
                    participacion7carrera5.Entity,
                    participacion8carrera5.Entity,
                    participacion9carrera5.Entity,
                    participacion10carrera5.Entity,
                    participacion11carrera5.Entity,
                    participacion12carrera5.Entity
                });

                // Helper local para generar participaciones masivas
                List<EntityEntry<Participacion>> GenerarParticipacionesParaCarrera(EntityEntry<Carrera> car, int cantidad, EstadoEvento estadoEvento)
                {
                    var todasPersonas = new List<EntityEntry<Participante>>
                    {
                        participante1, participante2, participante3, participante4, participante5,
                        participante6, participante7, participante8, participante9, participante10,
                        participante11, participante12
                    };
                    todasPersonas.AddRange(participantesExtras);

                    var result = new List<EntityEntry<Participacion>>();
                    for (int i = 0; i < cantidad; i++)
                    {
                        var persona = todasPersonas[i % todasPersonas.Count];
                        var numero = i + 1;
                        var estadoPart = estadoEvento switch
                        {
                            EstadoEvento.Finalizado => EstadoParticipanteEnCarrera.Completada,
                            EstadoEvento.EnCurso => EstadoParticipanteEnCarrera.EnCurso,
                            _ => EstadoParticipanteEnCarrera.SinComenzar
                        };

                        var progreso = new Dictionary<uint, TimeSpan>();
                        if (estadoEvento == EstadoEvento.Finalizado)
                        {
                            // Generar tiempos simples crecientes para PCs
                            uint pcMax = (uint)(car.Entity.PuntosDeControl?.Count ?? 0);
                            var baseMin = 30 + (i % 20); // Base para variar
                            for (uint pc = 1; pc <= pcMax; pc++)
                            {
                                progreso[pc] = TimeSpan.FromMinutes(baseMin + (pc * 10));
                            }
                        }
                        else if (estadoEvento == EstadoEvento.EnCurso)
                        {
                            uint pcMax = (uint)Math.Max(1, (car.Entity.PuntosDeControl?.Count ?? 1) - 1);
                            var baseMin = 15 + (i % 15);
                            for (uint pc = 1; pc <= pcMax; pc++)
                            {
                                progreso[pc] = TimeSpan.FromMinutes(baseMin + (pc * 8));
                            }
                        }

                        var part = context.Participaciones.Add(new Participacion
                        {
                            EventoId = car.Entity.EventoId,
                            ParticipanteId = persona.Entity.Id,
                            NumeroCorredor = (uint)numero,
                            Puesto = estadoEvento == EstadoEvento.Finalizado ? (uint)numero : 0u,
                            Estado = estadoPart,
                            Progreso = progreso,
                            EstadoPago = EstadoPago.Confirmado
                        });
                        
                        // Si la carrera esta finalizada mostrar el ganador con el tiempo del ultimo punto de control registrado
                        if (estadoEvento == EstadoEvento.Finalizado && numero == 1)
                        {
                            car.Entity.Ganador = $"{persona.Entity.Nombre} {persona.Entity.Apellido}";
                            if (progreso.Count > 0)
                            {
                                var tiempoFinal = progreso.OrderBy(kv => kv.Key).Last().Value;
                                car.Entity.TiempoGanador = tiempoFinal;
                            }
                            else
                            {
                                car.Entity.TiempoGanador = null;
                            }
                        }
                        // Asociar navegacion
                        persona.Entity.Carreras.Add(part.Entity);
                        car.Entity.Participaciones.Add(part.Entity);
                        result.Add(part);
                    }
                    return result;
                }

                // Generar participaciones para carreras nuevas con volúmenes 22, 28, 35, 40
                var partsC6 = GenerarParticipacionesParaCarrera(carrera6, 22, evento6.Entity.EstadoEvento);
                var partsC7 = GenerarParticipacionesParaCarrera(carrera7, 28, evento7.Entity.EstadoEvento);
                var partsC8 = GenerarParticipacionesParaCarrera(carrera8, 35, evento8.Entity.EstadoEvento);
                var partsC9 = GenerarParticipacionesParaCarrera(carrera9, 40, evento9.Entity.EstadoEvento);
            }

            int cambios = context.SaveChanges();

            if (cambios == 0)
            {
                Console.WriteLine("No se insertaron registros en la base de datos.");
                return;
            }
            Console.WriteLine($"Guardando datos de prueba... {cambios} cambios realizados.");
        }
    }

    /// <summary>
    /// Limpia todas las tablas de la base de datos sin dropear el esquema.
    /// Prioriza TRUNCATE con FOREIGN_KEY_CHECKS=0 (MySQL). Si falla, hace DELETE en orden seguro.
    /// </summary>
    public static void ClearAllTables(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("Iniciando limpieza de todas las tablas...");

        try
        {
            // MySQL: deshabilitar FKs y truncar para limpiar rápido
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE `Participaciones`;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE `PuntosDeControl`;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE `Carreras`;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE `Eventos`;");
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE `Usuarios`;");
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");

            Console.WriteLine("Limpieza por TRUNCATE completada.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TRUNCATE falló o no está soportado: {ex.Message}. Reintentando con DELETE...");
        }

        // Fallback universal: DELETE en orden respetando FKs
        // EF Core 7+ soporta ExecuteDelete() sin materializar entidades.
        try
        {
            context.Participaciones.ExecuteDelete();
            context.PuntosDeControl.ExecuteDelete();
            context.Carreras.ExecuteDelete();
            context.Eventos.ExecuteDelete();
            context.Usuarios.ExecuteDelete();
            Console.WriteLine("Limpieza por DELETE completada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al limpiar tablas con DELETE: {ex.Message}");
            throw;
        }
    }
}