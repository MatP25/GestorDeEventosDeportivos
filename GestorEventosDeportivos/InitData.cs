using GestorEventosDeportivos.Modules.Carreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using GestorEventosDeportivos.Modules.ProgresoCarreras.Domain.Entities;
using GestorEventosDeportivos.Modules.Usuarios.Domain.Entities;
using GestorEventosDeportivos.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

            EntityEntry<Evento> evento1, evento2, evento3, evento4;
            EntityEntry<Carrera> carrera1, carrera2, carrera3, carrera4;
            EntityEntry<Participante> participante1, participante2, participante3;
            EntityEntry<Participacion> participacion1carrera1, participacion2carrera1, participacion3carrera1,
                participacion1carrera2, participacion2carrera2, participacion3carrera2,
                participacion1carrera3, participacion2carrera3, participacion3carrera3,
                participacion1carrera4, participacion2carrera4, participacion3carrera4;

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
                    CantidadParticipantes = 10
                });

                evento2 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 2",
                    FechaInicio = DateTime.Now.AddMonths(1),
                    Ubicacion = "Maldonado",
                    RegistroHabilitado = true,
                    EstadoEvento = EstadoEvento.SinComenzar,
                    CantidadParticipantes = 10
                });

                evento3 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón 3",
                    FechaInicio = DateTime.Now.AddDays(-5),
                    Ubicacion = "Punta del Este",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.Finalizado,
                    CantidadParticipantes = 10
                });

                evento4 = context.Eventos.Add(new()
                {
                    Nombre = "Maratón en Curso",
                    FechaInicio = DateTime.Now,
                    Ubicacion = "Montevideo",
                    RegistroHabilitado = false,
                    EstadoEvento = EstadoEvento.EnCurso,
                    CantidadParticipantes = 10
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
                    }
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
                    }
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
                    }
                });

                carrera4 = context.Carreras.Add(new()
                {
                    EventoId = evento4.Entity.Id,
                    Longitud = 10000,
                    PuntosDeControl = new List<PuntoDeControl>
                    {
                        new PuntoDeControl { Posicion = 1, Ubicacion = "Punto 1 - Km 5" },
                        new PuntoDeControl { Posicion = 2, Ubicacion = "Punto 2 - Km 10" }
                    }
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
                    }
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
                        }
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
                        }
                    });

                participacion1carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante1.Entity.Id,
                        NumeroCorredor = 0,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { }
                    });

                participacion2carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante2.Entity.Id,
                        NumeroCorredor = 0,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { }
                    });

                participacion3carrera2 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera2.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = 0,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.SinComenzar,
                        Progreso = new Dictionary<uint, TimeSpan> { }
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
                        }
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
                        }
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
                        }
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
                        }
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
                        }
                    });

                participacion3carrera4 = context.Participaciones.Add(
                    new Participacion
                    {
                        EventoId = carrera4.Entity.EventoId,
                        ParticipanteId = participante3.Entity.Id,
                        NumeroCorredor = 103,
                        Puesto = 0,
                        Estado = EstadoParticipanteEnCarrera.EnCurso,
                        Progreso = new Dictionary<uint, TimeSpan> { }
                    });

                participante1.Entity.Carreras.AddRange(new[]
                {
                    participacion1carrera1.Entity,
                    participacion1carrera2.Entity,
                    participacion1carrera3.Entity,
                    participacion1carrera4.Entity
                });

                participante2.Entity.Carreras.AddRange(new[]
                {
                    participacion2carrera1.Entity,
                    participacion2carrera2.Entity,
                    participacion2carrera3.Entity,
                    participacion2carrera4.Entity
                });

                participante3.Entity.Carreras.AddRange(new[]
                {
                    participacion3carrera1.Entity,
                    participacion3carrera2.Entity,
                    participacion3carrera3.Entity,
                    participacion3carrera4.Entity
                });

                carrera1.Entity.Participaciones.AddRange(new[]
                {
                    participacion1carrera1.Entity,
                    participacion2carrera1.Entity,
                    participacion3carrera1.Entity
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
}