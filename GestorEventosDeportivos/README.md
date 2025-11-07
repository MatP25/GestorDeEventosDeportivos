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

### Simulador de carrera normal directo EF Core

#### Obtener el GUID de carreras con estado Sin comenzar

- dotnet run --project Simulator/RaceSimulator.csproj -- --list


#### Comenzar la simulacion de la carrera 
- dropout -> abandono / dq -> descalificado / duration -> duracion / seed -> semilla que si usas los mismos datos vas a obtener resultados iguales o similares. si usas los mismos datos y quieres resultados diferentes cambiar el seed.

- dotnet run --project Simulator/RaceSimulator.csproj -- --carrera GUID --duration 60 --dropout 0.10 --dq 0.10 --seed 42

#### Reiniciar la carrera (vuelve todo a como estaba antes de la carrera)

- dotnet run --project Simulator/RaceSimulator.csproj -- --reset GUID

### Simulador de carrera por balanceador (http-sim)

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

- Asegurar de que `docker compose -f docker-compose_loadB.yml up -d` este corriendo
- Para verificar que todo ande previo a la simulacion se puede usar lo siguiente: 
```sh
dotnet run --project Simulator/RaceSimulator.csproj -- --http-mode --requests 10 --concurrency 3 --lb-url http://localhost:8080
```

--- 

## Simulación de pago de inscripción desde servicio externo:

El id de pago es un código generado para cada usuario y que se le muestra en el perfil. El código es generado a partir de la encriptación y codificación del Guid del usuario en la base de datos, por lo que se evita hacer público el identificador real.

**Linux**

```curl -X POST http://localhost:5195/api/carreras/{id carrera}/pagos/{id de pago}```

**Windows**

```Invoke-WebRequest -Method Post -Uri http://localhost:5195/api/carreras/{id carrera}/pagos/{id de pago}```

