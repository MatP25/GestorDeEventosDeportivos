## Plan de pruebas

### Generar imagen lectura sensor y generar las 3 instancias de app(app 1-2-3)

1. ```dotnet build```

2. ```dotnet publish -c Release -o ./publish```

3. ```docker build -t lectura-sensor .```

4. ```docker compose -f docker-compose_loadB.yml up -d --force-recreate```

###                     Simulador

5. dotnet run --project Simulator/RaceSimulator.csproj -- --list

6. dotnet run --project Simulator/RaceSimulator.csproj -- \
	--http-sim \
	--carrera GUID \
	--duration 120 \
	--dropout 0.10 \
	--dq 0.10 \
	--seed 42 \
	--lb-url http://localhost:8080

7. dotnet run --project Simulator/RaceSimulator.csproj -- --reset GUID

## Prueba basica

1. Login admin
    1. Crear Evento
    2. Crear Carrera
    3. Añadir puntos de control
2. Login User
    1. Inscribir a la carrera
    2. Pagar la inscripción
    3. (Opcional) Login Admin
    4. Aprobar Inscripcion del usuario ID..
3. Ejecutar Simulador de carreras
    1. Navegar hacia la carrera en curso
    2. Observar la carrera en curso mediante signalR
