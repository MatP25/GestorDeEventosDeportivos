# Gestor De Eventos Deportivos

## Resumen

Aplicación web (ASP.NET Core 9 + Blazor Server) para gestionar carreras y participantes, con actualización de progreso en tiempo real, asignación de puestos y soporte para un simulador de carreras. Base de datos **MySQL** 8 con EF Core 9 (Pomelo). Despliegue local con Docker Compose y balanceador **NGINX** frente a tres instancias de la app.

## Dominio de clases

[![](https://mermaid.ink/img/pako:eNq9Vltv2jAU_ivID3uYABFaLk0rqgpoV6mlqJeXKS8mPqTWEjuynW0t5b_v5ELjhPQibRov2P7Oxd-5ORviSwbEJX5ItZ5xGigaeaKFv-ykNf8JwsjWJj9Lf5es5bYuEs7Ko7k2lEk8zhe5TgkvZLRSgPCdUVwEJXAO_iO9FNznqfKMGrjnEZT4lMbU54yyJVUGpWIqDGgUTbgwpdgtBFwbJb_RFQ95cZWVlGEusvWEzWhKlQJFP6KU83aLxWmJXFCBHhRCOqNjQfccoliWAimbO7y0JXIlRcBNwvZI-MitQhWDIkAvaUD3GdeEEL_CCHwJTQWYWAoJktAzmEqBgQothSowaQxZJlQqvx-5pdRpQsXepR9W3KcFUq-EIikN5nbpcner0-Yr2sQ_ldsM2MuuXWlN5CwYQXtrGVkkESg5lXjfvBDSQNhOEtBZcVUDVGsj2_hcFOzr0lgftkZgtd1SyQB01lncNxgWqp4w48ep0_ZrbTZn_EEnVPGP-v61sXUtnWcxhCHPuNSheUR52HC-RLe_pGINUDYmFtQvZsSNCJ9KEE-kXoJaZ1bzRXH72f3N-7WSJnGzV2pWOx039dPWtnXGIi7S6ZO2_M7NLnonL51OxVsD3GhgV_SdzqTlEccj5TyKQRkQ4EOrqIUCKUS73a8oXTaN4Si8M1vr49J6Kb9nvnaV3H7NUN1NpRf_wkvFzied1DoUhyODmkaK5Aq9Tzgq3kHrZbNr5uRkvni4nlij9o6LqYxAPFNlVb2YJkpbzXnOBQ35M9psLFGrud91tpC3ULGTd5-Sq4zLEgRDLsZ-U6VYcxV95Lhh9PwL2igSh4AurEF2tqKC4XSyz2agfeS1xhfjjYvWO_0_jqo359GWtEmgOCOuUQm0Cb4CaAK3JLucR8wj4PcNcXHJqPrhEU-kOhjn71JGOzUlk-CRuGsaatwlMUMnxafZ66nCzIKaSmxE4jrDg6PMCnE35DfunVHX6Q-ODvvD0bDfG4zHbfJE3KN-t9dzxs5gfDgYjAcH422bPGd-e93ReDhyhmgoRZ3hqE2AcSPVdfF5mP5t_wCUIxrc?type=png)](https://mermaid.live/edit#pako:eNq9Vltv2jAU_ivID3uYABFaLk0rqgpoV6mlqJeXKS8mPqTWEjuynW0t5b_v5ELjhPQibRov2P7Oxd-5ORviSwbEJX5ItZ5xGigaeaKFv-ykNf8JwsjWJj9Lf5es5bYuEs7Ko7k2lEk8zhe5TgkvZLRSgPCdUVwEJXAO_iO9FNznqfKMGrjnEZT4lMbU54yyJVUGpWIqDGgUTbgwpdgtBFwbJb_RFQ95cZWVlGEusvWEzWhKlQJFP6KU83aLxWmJXFCBHhRCOqNjQfccoliWAimbO7y0JXIlRcBNwvZI-MitQhWDIkAvaUD3GdeEEL_CCHwJTQWYWAoJktAzmEqBgQothSowaQxZJlQqvx-5pdRpQsXepR9W3KcFUq-EIikN5nbpcner0-Yr2sQ_ldsM2MuuXWlN5CwYQXtrGVkkESg5lXjfvBDSQNhOEtBZcVUDVGsj2_hcFOzr0lgftkZgtd1SyQB01lncNxgWqp4w48ep0_ZrbTZn_EEnVPGP-v61sXUtnWcxhCHPuNSheUR52HC-RLe_pGINUDYmFtQvZsSNCJ9KEE-kXoJaZ1bzRXH72f3N-7WSJnGzV2pWOx039dPWtnXGIi7S6ZO2_M7NLnonL51OxVsD3GhgV_SdzqTlEccj5TyKQRkQ4EOrqIUCKUS73a8oXTaN4Si8M1vr49J6Kb9nvnaV3H7NUN1NpRf_wkvFzied1DoUhyODmkaK5Aq9Tzgq3kHrZbNr5uRkvni4nlij9o6LqYxAPFNlVb2YJkpbzXnOBQ35M9psLFGrud91tpC3ULGTd5-Sq4zLEgRDLsZ-U6VYcxV95Lhh9PwL2igSh4AurEF2tqKC4XSyz2agfeS1xhfjjYvWO_0_jqo359GWtEmgOCOuUQm0Cb4CaAK3JLucR8wj4PcNcXHJqPrhEU-kOhjn71JGOzUlk-CRuGsaatwlMUMnxafZ66nCzIKaSmxE4jrDg6PMCnE35DfunVHX6Q-ODvvD0bDfG4zHbfJE3KN-t9dzxs5gfDgYjAcH422bPGd-e93ReDhyhmgoRZ3hqE2AcSPVdfF5mP5t_wCUIxrc)

## Módulo de componentes

### Flujo
1. El navegador establece una sesion interactiva (Blazor server) y conexiones signalR para tiempo real.
2. El simulador en modo HTTP consume las APIs expuestas por cualquier instancia (segun decida el balanceador).
3. Cada instancia modifica la misma DB, los cambios se reflejan globalmente.
4. El servicio RaceWatcher de cada instancia observan la BD en busca de cambios en el progreso/estado de las carreras en curso.

[![](https://mermaid.ink/img/pako:eNqtUdFu2jAU_RXrPlENWHACdqMKCWi1IVGWJkhMw30wiUuiJnbkONtaxL_XJu02ra99ss_1Ocfn3nuEVGUCQngo1a8059qgVcwkkwjN42_b5CbeMZiX_FlplAj9U-irvZ4uykJIIxq0FXsG9-hqMEAMvm420WdbSVT6KAwDNBhM0frLcv191zsfaDW_uHfWyfLW2sY8FUlRtSU31n2xWjqrP5ouRNPuD5rXOZpFUWM1s7pGPR8VsjFcpgVvLqzIEZFjjBwjiYbrmw1aKC1c1tfwn1BSHCQvY3ubRcvGPbkAW27SXGjXW5GKf83wR5r5H2QmZNYNphuom5br-10Fv6v4ne56vuvdPiV3q24TTuzWN7UPrxj_h_03DH046CKD0OhW9KESuuIOwtExGZhcVDZnaK8Z148MmDxZTc3lD6WqN5lW7SGH8IGXjUVtnXEjrgtut_yXYrsUeqFaaSAckbMFhEf4DSHxhphQQrwgmBBC_KAPT5bjjYd4MhkFPrmkFNMRPvXh-fypN6TBhJKxh8eU0iC4xKcX1XHkTA?type=png)](https://mermaid.live/edit#pako:eNqtUdFu2jAU_RXrPlENWHACdqMKCWi1IVGWJkhMw30wiUuiJnbkONtaxL_XJu02ra99ss_1Ocfn3nuEVGUCQngo1a8059qgVcwkkwjN42_b5CbeMZiX_FlplAj9U-irvZ4uykJIIxq0FXsG9-hqMEAMvm420WdbSVT6KAwDNBhM0frLcv191zsfaDW_uHfWyfLW2sY8FUlRtSU31n2xWjqrP5ouRNPuD5rXOZpFUWM1s7pGPR8VsjFcpgVvLqzIEZFjjBwjiYbrmw1aKC1c1tfwn1BSHCQvY3ubRcvGPbkAW27SXGjXW5GKf83wR5r5H2QmZNYNphuom5br-10Fv6v4ne56vuvdPiV3q24TTuzWN7UPrxj_h_03DH046CKD0OhW9KESuuIOwtExGZhcVDZnaK8Z148MmDxZTc3lD6WqN5lW7SGH8IGXjUVtnXEjrgtut_yXYrsUeqFaaSAckbMFhEf4DSHxhphQQrwgmBBC_KAPT5bjjYd4MhkFPrmkFNMRPvXh-fypN6TBhJKxh8eU0iC4xKcX1XHkTA)


### Flujo (unica instancia)
1. El cliente navega a detalles de carrera, La aplicacion crea una conexion en tiempo real con (SignalR) con el balanceador de carga de por medio.
3. El cliente entra a una carrera en especifico
4. El simulador envia cambios  a la api de progreso pasando por el balanceador de carga. La app valida los datos y los guarda en la BD.
5. Al guardarse los datos, los cambios quedan disponibles para todas las instancias porque comparten la misma BD
6. Mientras tanto el proceso en segundo plano RaceWatcherService) se ejecuta cada 2 segundos y lee el estado de la carrera y los participantes en la BD.
7. Si detecta algun cambio envia un mensaje RaceUpdated por signalR al grupo de esa carrera
8. Con varias instancias (usamos 3) cualquiera puede detectar y avisar, el usuario lo vera igual porque la base de datos es unica para todas.
9. Dentro de la instancia: UI -> Servicios -> EF Core -> BD. El servicio RaceWatcher observa la bd y usa hubs.

[![](https://mermaid.ink/img/pako:eNptU1lu2zAQvQpBIIjdek1SL6prwFsbA47rSs6CxvmgpbFMRCYFSkriBAF6iF6gZ-gRcpOepENKlhO0P6LmzfI4b4ZP1JUeUIuuAnnvrpmKycReiIUgxA1YFA1hRYS8Vywk92seQzkKmQtWCn1MAw8OyCDgIGKItBklSx-dazKYjEfT-eh6QTP3gt7oAEL69tdLZ2SjZ8ruwGeeVJ2l6hb6AXuUijig7kCR83ExywDh5VR9FjDhgs7RyPTLeHp1XTCHrvHKTTwgLlM-K97k2Q7fJIFxwkMMSkiNO-MzvIqNjaXuGN14eUOe5fXCgLvM5S-_BSmTRDDCRRQjE2dpS3nXvdkMi3VQJ0GieBvAp8N_lTvsvi5YqExHc1Ilb9ovkj8_fpLxjqVT1RW7uYQeV-DGXAoy76eImZemzwaWovZA6y83oRRmQsTWHEbt2csvnwsWkfdkwrYyiaNiXv70vO9gosN9wQKbnCbLSOeUiVbpPPQYlkIwxS5A8RV2o6-DYF6kNxtjDfxG2NxAiljJIACVVaqykFdDJX0Fkay-y8A7XWtbQm8Sr6vv8lrOhW5ES8NdLiM9W7aXME0eMKVAsTQIUmyWEWRgxn0eJUzxHNy3rZs-lVEMXuYjhT5zb30lE-EV9xJcsthdg8qC8vxhfzC_whKjz9ivMlfoheFwqZvHhfvPOs9QD458u00a9q8LZ1vn2-TV0mI2PKC2YNpW4Gk8e0OkUy4TvPN8PqtewtKR7i0gDymXu-nbyBb8LWB-DIT7kvPYEKCchocL_TqYIQwYSm3WyR5ouq7ZjsxOa4y1hUfqxllp-9Qhryw8Uq_RKG0Vf3bYQtAS9RX3qBWrBEp0A2rDtEmfdPCCxmvYoNIW_npM3S7oQjxjDr6K71Judmk4KH9NrRULIrQSs6lDzvBt7kNQf1ADnGhMrUa9bmpQ64k-UKt8Ujk6PmrW6s1m66RebzU-lOiWWq1apXXcPG41a62TdqPWPnku0UfDWqu0mxjbbLTb7aPGUb3eeP4L4LG26A?type=png)](https://mermaid.live/edit#pako:eNptU1lu2zAQvQpBIIjdek1SL6prwFsbA47rSs6CxvmgpbFMRCYFSkriBAF6iF6gZ-gRcpOepENKlhO0P6LmzfI4b4ZP1JUeUIuuAnnvrpmKycReiIUgxA1YFA1hRYS8Vywk92seQzkKmQtWCn1MAw8OyCDgIGKItBklSx-dazKYjEfT-eh6QTP3gt7oAEL69tdLZ2SjZ8ruwGeeVJ2l6hb6AXuUijig7kCR83ExywDh5VR9FjDhgs7RyPTLeHp1XTCHrvHKTTwgLlM-K97k2Q7fJIFxwkMMSkiNO-MzvIqNjaXuGN14eUOe5fXCgLvM5S-_BSmTRDDCRRQjE2dpS3nXvdkMi3VQJ0GieBvAp8N_lTvsvi5YqExHc1Ilb9ovkj8_fpLxjqVT1RW7uYQeV-DGXAoy76eImZemzwaWovZA6y83oRRmQsTWHEbt2csvnwsWkfdkwrYyiaNiXv70vO9gosN9wQKbnCbLSOeUiVbpPPQYlkIwxS5A8RV2o6-DYF6kNxtjDfxG2NxAiljJIACVVaqykFdDJX0Fkay-y8A7XWtbQm8Sr6vv8lrOhW5ES8NdLiM9W7aXME0eMKVAsTQIUmyWEWRgxn0eJUzxHNy3rZs-lVEMXuYjhT5zb30lE-EV9xJcsthdg8qC8vxhfzC_whKjz9ivMlfoheFwqZvHhfvPOs9QD458u00a9q8LZ1vn2-TV0mI2PKC2YNpW4Gk8e0OkUy4TvPN8PqtewtKR7i0gDymXu-nbyBb8LWB-DIT7kvPYEKCchocL_TqYIQwYSm3WyR5ouq7ZjsxOa4y1hUfqxllp-9Qhryw8Uq_RKG0Vf3bYQtAS9RX3qBWrBEp0A2rDtEmfdPCCxmvYoNIW_npM3S7oQjxjDr6K71Judmk4KH9NrRULIrQSs6lDzvBt7kNQf1ADnGhMrUa9bmpQ64k-UKt8Ujk6PmrW6s1m66RebzU-lOiWWq1apXXcPG41a62TdqPWPnku0UfDWqu0mxjbbLTb7aPGUb3eeP4L4LG26A)

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


