# Gestor De Eventos Deportivos

## Resumen

Aplicación web (ASP.NET Core 9 + Blazor Server) para gestionar carreras y participantes, con actualización de progreso en tiempo real, asignación de puestos y soporte para un simulador de carreras. Base de datos **MySQL** 8 con EF Core 9 (Pomelo). Despliegue local con Docker Compose y balanceador **NGINX** frente a tres instancias de la app.

## Dominio de clases

[![](https://mermaid.ink/img/pako:eNq9Vltv2jAU_ivID3uYABFaLk0rqgpoV6mlqJeXKS8mPqTWEjuynW0t5b_v5ELjhPQibRov2P7Oxd-5ORviSwbEJX5ItZ5xGigaeaKFv-ykNf8JwsjWJj9Lf5es5bYuEs7Ko7k2lEk8zhe5TgkvZLRSgPCdUVwEJXAO_iO9FNznqfKMGrjnEZT4lMbU54yyJVUGpWIqDGgUTbgwpdgtBFwbJb_RFQ95cZWVlGEusvWEzWhKlQJFP6KU83aLxWmJXFCBHhRCOqNjQfccoliWAimbO7y0JXIlRcBNwvZI-MitQhWDIkAvaUD3GdeEEL_CCHwJTQWYWAoJktAzmEqBgQothSowaQxZJlQqvx-5pdRpQsXepR9W3KcFUq-EIikN5nbpcner0-Yr2sQ_ldsM2MuuXWlN5CwYQXtrGVkkESg5lXjfvBDSQNhOEtBZcVUDVGsj2_hcFOzr0lgftkZgtd1SyQB01lncNxgWqp4w48ep0_ZrbTZn_EEnVPGP-v61sXUtnWcxhCHPuNSheUR52HC-RLe_pGINUDYmFtQvZsSNCJ9KEE-kXoJaZ1bzRXH72f3N-7WSJnGzV2pWOx039dPWtnXGIi7S6ZO2_M7NLnonL51OxVsD3GhgV_SdzqTlEccj5TyKQRkQ4EOrqIUCKUS73a8oXTaN4Si8M1vr49J6Kb9nvnaV3H7NUN1NpRf_wkvFzied1DoUhyODmkaK5Aq9Tzgq3kHrZbNr5uRkvni4nlij9o6LqYxAPFNlVb2YJkpbzXnOBQ35M9psLFGrud91tpC3ULGTd5-Sq4zLEgRDLsZ-U6VYcxV95Lhh9PwL2igSh4AurEF2tqKC4XSyz2agfeS1xhfjjYvWO_0_jqo359GWtEmgOCOuUQm0Cb4CaAK3JLucR8wj4PcNcXHJqPrhEU-kOhjn71JGOzUlk-CRuGsaatwlMUMnxafZ66nCzIKaSmxE4jrDg6PMCnE35DfunVHX6Q-ODvvD0bDfG4zHbfJE3KN-t9dzxs5gfDgYjAcH422bPGd-e93ReDhyhmgoRZ3hqE2AcSPVdfF5mP5t_wCUIxrc?type=png)](https://mermaid.live/edit#pako:eNq9Vltv2jAU_ivID3uYABFaLk0rqgpoV6mlqJeXKS8mPqTWEjuynW0t5b_v5ELjhPQibRov2P7Oxd-5ORviSwbEJX5ItZ5xGigaeaKFv-ykNf8JwsjWJj9Lf5es5bYuEs7Ko7k2lEk8zhe5TgkvZLRSgPCdUVwEJXAO_iO9FNznqfKMGrjnEZT4lMbU54yyJVUGpWIqDGgUTbgwpdgtBFwbJb_RFQ95cZWVlGEusvWEzWhKlQJFP6KU83aLxWmJXFCBHhRCOqNjQfccoliWAimbO7y0JXIlRcBNwvZI-MitQhWDIkAvaUD3GdeEEL_CCHwJTQWYWAoJktAzmEqBgQothSowaQxZJlQqvx-5pdRpQsXepR9W3KcFUq-EIikN5nbpcner0-Yr2sQ_ldsM2MuuXWlN5CwYQXtrGVkkESg5lXjfvBDSQNhOEtBZcVUDVGsj2_hcFOzr0lgftkZgtd1SyQB01lncNxgWqp4w48ep0_ZrbTZn_EEnVPGP-v61sXUtnWcxhCHPuNSheUR52HC-RLe_pGINUDYmFtQvZsSNCJ9KEE-kXoJaZ1bzRXH72f3N-7WSJnGzV2pWOx039dPWtnXGIi7S6ZO2_M7NLnonL51OxVsD3GhgV_SdzqTlEccj5TyKQRkQ4EOrqIUCKUS73a8oXTaN4Si8M1vr49J6Kb9nvnaV3H7NUN1NpRf_wkvFzied1DoUhyODmkaK5Aq9Tzgq3kHrZbNr5uRkvni4nlij9o6LqYxAPFNlVb2YJkpbzXnOBQ35M9psLFGrud91tpC3ULGTd5-Sq4zLEgRDLsZ-U6VYcxV95Lhh9PwL2igSh4AurEF2tqKC4XSyz2agfeS1xhfjjYvWO_0_jqo359GWtEmgOCOuUQm0Cb4CaAK3JLucR8wj4PcNcXHJqPrhEU-kOhjn71JGOzUlk-CRuGsaatwlMUMnxafZ66nCzIKaSmxE4jrDg6PMCnE35DfunVHX6Q-ODvvD0bDfG4zHbfJE3KN-t9dzxs5gfDgYjAcH422bPGd-e93ReDhyhmgoRZ3hqE2AcSPVdfF5mP5t_wCUIxrc)

## Módulo de componentes

- El usuario abre la app en el navegador (Blazor Server UI) y se conecta a NGINX.
- NGINX balancea las solicitudes entre varias instancias de la app ASP.NET Core: app1, app2 y app3.
- La UI de Blazor usa:
	- HTTP/HTTPS para cargar páginas y llamar a Minimal APIs.
	- WebSockets para la sesión interactiva de Blazor Server y para SignalR (tiempo real), todo pasando por NGINX.
- Cada instancia (app1/2/3) ejecuta Blazor + Minimal APIs + SignalR y se conecta a la misma base de datos MySQL.
- El simulador de carrera también llama por HTTP a NGINX, que lo distribuye a las instancias.

[![](https://mermaid.ink/img/pako:eNqdk11vmzAUhv-Kda5ajdAYwkdRVQnSaYuURFlJ1Wn1LhxwCSrYyEC3Nsp_n8FtErqrzVd-j9_nnOMD3kEiUgYBPBbiV7KlskHzW8KRWnW7ySSttmha5Iw3TEe7Fd3NHghEBX0VEsVMPjOJ7mZXG3lxfcbpM8toKuQ5gZ8aYTwlXG_j2UKRcV62RedBKUMJlZJJqulE8FoUDNGq0rzG5pGill9my-_atqEF5Qk7lvmr47ZuVFNhvDKXn9doKuRJ-yFW2VQFrJO93eMTWuQ8L2mBwtWsVjLOM06L2-M9etbSrPU_rK1Z-x_Zk_ndRA9nBBYv8bc5gfPDxdUHQaMR-rpery7u2SYWyRNrahW6VqM7TP7dMojPo16GeCitobS1DLGSI9XFm7SG0h5KjK5GpqJVdwf_x4h9GgEDMpmnEDSyZQaUTJa0k7Dr3ASaLSsZgUBtUyqfCBC-V0xF-Q8hyndMijbbQvBIi1qptkppw25yqv6Mo0WNlMmpaHkDAXYndp8Egh38Vhp7Jracy4nleq41dnzfgBcILi1zPMY-dvyJ4_iO7e8NeO3Ljk3Pdz3sYtfuTrHrGcDSvBFyod9W_8T2fwCLiPkX?type=png)](https://mermaid.live/edit#pako:eNqdk11vmzAUhv-Kda5ajdAYwkdRVQnSaYuURFlJ1Wn1LhxwCSrYyEC3Nsp_n8FtErqrzVd-j9_nnOMD3kEiUgYBPBbiV7KlskHzW8KRWnW7ySSttmha5Iw3TEe7Fd3NHghEBX0VEsVMPjOJ7mZXG3lxfcbpM8toKuQ5gZ8aYTwlXG_j2UKRcV62RedBKUMJlZJJqulE8FoUDNGq0rzG5pGill9my-_atqEF5Qk7lvmr47ZuVFNhvDKXn9doKuRJ-yFW2VQFrJO93eMTWuQ8L2mBwtWsVjLOM06L2-M9etbSrPU_rK1Z-x_Zk_ndRA9nBBYv8bc5gfPDxdUHQaMR-rpery7u2SYWyRNrahW6VqM7TP7dMojPo16GeCitobS1DLGSI9XFm7SG0h5KjK5GpqJVdwf_x4h9GgEDMpmnEDSyZQaUTJa0k7Dr3ASaLSsZgUBtUyqfCBC-V0xF-Q8hyndMijbbQvBIi1qptkppw25yqv6Mo0WNlMmpaHkDAXYndp8Egh38Vhp7Jracy4nleq41dnzfgBcILi1zPMY-dvyJ4_iO7e8NeO3Ljk3Pdz3sYtfuTrHrGcDSvBFyod9W_8T2fwCLiPkX)

## Arquitectura general

El sistema es un monolito.

- **Módulo Carreras**: Administra carreras y sus puntos de control (Creacion, estado general).
- **Módulo Progreso de Carreras**: Registra lecturas de corredores, calcula puestos y estados (Finaliza, Abanadona, Descalificado).
- **Módulo usuarios**: Login, roles (Admin/Participante) y datos basicos de usuarios
- **Módulo Verificación de Email**: Genera/Valida un token de verificación y notifica en pantalla cuando se verifica.
- **Servicios comunes**: Acceso a base de datos, endpoints web y comunicacion en tiempo real.

## Tecnologías utilizadas

- .NET 9 (ASP.NET Core) y Blazor Server para la interfaz.
- Base de datos MySql.
- Entity Framework Core.
- SignalR para actualizaciones en tiempo real.
- Docker y NGINX para levantar varias instancias y balancear carga.

## Estructura de Modulos

### Modulo de Usuarios
- **Entidades**: Usuario, Administrador, Participante
- **Funcionalidades**: Gestion de usuarios y roles

### Modulo de Carreras
- **Entidades**: Evento, Carrera, PuntoDeControl, Participacion
- **Funcionalidades**: Gestion de eventos deportivos y carreras

### Modulo de Progreso de Carreras
- **Funcionalidades**: Seguimiento del progreso de los participantes

## Explicacion de cada módulo

### Módulo Carreras

Se encarga de todo lo relacionado con la definición y administración de una carrera:
- Crea carreras con sus puntos de control.
- Mantiene el estado general (Sin comenzar, En curso, Finalizado) a traves del evento asociado.

### Módulo Progreso de Carreras

Se ocupa de registrar el avance real de cada participante:
- Guarda el tiempo de paso por cada punto de control (CP1, CP2, CP3... Meta) en orden.
- Decide cuándo un corredor terminó la carrera, abondonó o fue descalificado.

### Módulo Usuarios

Mmaneja la identidad y permisos básicos:
- Registra y autentica usuarios con email y password.
- Distingue roles: Administrador (gestiona carreras) y Participante.

### Módulo de verificación de Email
- Genera un token de verificación y ofrece una rota que lo consume (SImula como si el usuario hubiese hecho click en el enlace).
- Al verificar, avisa al usuario en tiempo real (SignalR).
- Marca al usuario como verificado para futuras acciones.


## Simulador de carrera

#### ¿Para que sirve?

- Simula una carrera generando lecturas de puntos de control y cambios de estados.
- Comprueba que la aplicación calcula tiempos y puestos ademas de probar la efectividad del balanceador de carga entre 3 instancias.

#### ¿Al finalizar el simulador que se muestra?

- Mensajes por cada punto de control: "Runner X CPi -> OK.
- Si alguien abandona o es descalificado.
- Resumen de cuantos corredores terminaron, abandonaron o fueron descalificados.
- Si se utiliza el balanceador de carga se muestra información sobre a cual instancia fue cada request.

#### ¿Como usarlo?

- Listar carreras disponibles:
    - ```dotnet run --project Simulator/RaceSimulator.csproj -- --list```
- Correr una simulación (Sin balanceador de carga):
    - ```dotnet run --project Simulator/RaceSimulator.csproj -- --carrera GUID --duration 60 --dropout 0.10 --dq 0.10 --seed 42```

    - En lineas diferentes
```sh
dotnet run --project Simulator/RaceSimulator.csproj -- \
	--http-sim \
	--carrera GUID \
	--duration 120 \
	--dropout 0.10 \
	--dq 0.10 \
	--seed 42
```
- Correr una simulación (Con balanceador de carga):
    - ```dotnet run --project Simulator/RaceSimulator.csproj -- --http-sim --carrera GUID --duration 60 --dropout 0.10 --dq 0.10 --seed 42 --lb-url http://localhost:8080```

    - En lineas diferentes
```sh
dotnet run --project Simulator/RaceSimulator.csproj -- \
	--http-sim \
	--carrera GUID \
	--duration 120 \
	--dropout 0.10 \
	--dq 0.10 \
	--seed 42 \
	--lb-url http://localhost:8080
```
- Reiniciar la carrera (vuelve todo a como estaba antes de la carrera(estado carrera y participantes))

    - ```dotnet run --project Simulator/RaceSimulator.csproj -- --reset GUID```

#### Información

- dropout: es el porcentaje que abandona.
- dq: es el porcentaje que son descalificados.
- duration: es la duración media de la carrera en segundos.
- seed: Ayuda a repetir resultados similares usando los mismos datos.


## Balanceador de carga (NGINX)

### Generar imagen lectura-sensor 

1. ```dotnet build```

2. ```dotnet publish -c Release -o ./publish```

3. ```docker build -t lectura-sensor .```

4. Verificamos que se haya creado la imagen -> ```docker images | grep lectura-sensor```

5. ```docker compose -f docker-compose_loadB.yml up -d```

### Recrear imagen y contenedores

```docker compose -f docker-compose_loadB.yml up -d --force-recreate```

### Ver logs

```
docker logs -f gestoreventosdeportivos-app3-1
docker logs -f gestoreventosdeportivos-app1-1
docker logs -f gestoreventosdeportivos-app2-1
docker logs -f gestoreventosdeportivos-nginx-1
```

### Limpiar contenedores e imagen

#### Detiene los contenedores que se basan en la imagen lectura-sensor
- ```docker ps -a --filter ancestor=lectura-sensor -q | xargs -r docker stop```

#### Elimina los contenedores de la imagen lectura-sensor
- ```docker ps -a --filter ancestor=lectura-sensor -q | xargs -r docker rm```

#### Elimina la imagen lectura-sensor
- ```docker rmi lectura-sensor```

### Probar

```for i in {1..20}; do curl -s http://localhost:8080/api/progreso/generar-lectura > /dev/null; done```

### Comando extra

```dotnet run -- --urls http://0.0.0.0:5195```

```dotnet watch run -- --urls http://0.0.0.0:5195```

---

## Instalación y Configuracion

### 1. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd GestorDeEventosDeportivos
```

### 2. Configurar la Base de Datos con Docker

Iniciar el contenedor de MySQL:

```bash
cd GestorEventosDeportivos/Docker
docker-compose up -d
```

### 3. Verificar la Conexion a la Base de Datos

```bash
# Verifica que el contenedor se esta ejecutando
docker ps

# Conectarse a MySQL 
docker exec -it gestor_eventos_mysql mysql -u appuser -p
```

### 4. Instalar Dependencias y Ejecutar Migraciones

```bash
1. dotnet tool install --global dotnet-ef # Preparar el CLI de EF por si no se tiene
2. dotnet restore
3. dotnet ef migrations add InitialCreate
4. dotnet ef database update
```

### 5. Ejecutar la Aplicacion

```bash
dotnet run
```

## Comandos de Entity Framework

### Crear una Nueva Migracion

```bash
dotnet ef migrations add <NombreDeLaMigracion>
```

### Aplicar Migraciones

```bash
dotnet ef database update
```

### Eliminar la Ultima Migracion

```bash
dotnet ef migrations remove
```

### Ver el Estado de las Migraciones

```bash
dotnet ef migrations list
```

## Comandos de Docker

### Gestion del Contenedor MySQL

```bash
# Iniciar los servicios
docker-compose up -d

# Detener los servicios
docker-compose down

# Ver logs del contenedor
docker-compose logs mysql

# Reiniciar el contenedor
docker-compose restart mysql

# Eliminar contenedor y volumenes
docker-compose down -v
```


