mkdir data
echo {"DockerHost": "192.168.200.11"} > data/Settings.json

docker build -t docker_manager -f docker_manager\Dockerfile .
docker stop test
docker rm test
docker run -p 1337:1337 -d -v %cd%/data:/app/data --name test docker_manager
