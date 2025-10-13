# Gestor De Eventos Deportivos


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

## Estructura de Modulos

### Modulo de Usuarios
- **Entidades**: Usuario, Administrador, Participante
- **Funcionalidades**: Gestion de usuarios y roles

### Modulo de Carreras
- **Entidades**: Evento, Carrera, PuntoDeControl, Participacion
- **Funcionalidades**: Gestion de eventos deportivos y carreras

### Modulo de Progreso de Carreras
- **Funcionalidades**: Seguimiento del progreso de los participantes
