echo "Iniciando contenedores docker... paciencia bro"

cd /Docker
docker compose up -d

cd ..

docker compose -f docker-compose_loadB.yml up -d --force-recreate

