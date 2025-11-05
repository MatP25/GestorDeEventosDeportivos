## Generar imagen lectura-sensor 

1. ```dotnet build```

2. ```dotnet publish -c Release -o ./publish```

3. ```docker build -t lectura-sensor .```

4. Verificamos que se haya creado la imagen -> ```docker images | grep lectura-sensor```

5. ```docker compose -f docker-compose_loadB.yml up -d```

## Recrear imagen y contenedores

```docker compose -f docker-compose_loadB.yml up -d --force-recreate```

### Ver logs

```
docker logs -f gestoreventosdeportivos-app3-1
docker logs -f gestoreventosdeportivos-app1-1
docker logs -f gestoreventosdeportivos-app2-1
docker logs -f gestoreventosdeportivos-nginx-1
```

### Probar

```for i in {1..20}; do curl -s http://localhost:8080/api/progreso/generar-lectura > /dev/null; done```

### Comando extra

```dotnet run -- --urls http://0.0.0.0:5195```

```dotnet watch run -- --urls http://0.0.0.0:5195```

--- 

## Simulación de pago de inscripción desde servicio externo:

El id de pago es un código generado para cada usuario y que se le muestra en el perfil. El código es generado a partir de la encriptación y codificación del Guid del usuario en la base de datos, por lo que se evita hacer público el identificador real.

**Linux**

```curl -X POST http://localhost:5195/api/carreras/{id carrera}/pagos/{id de pago}```

**Windows**

```Invoke-WebRequest -Method Post -Uri http://localhost:5195/api/carreras/{id carrera}/pagos/{id de pago}```

