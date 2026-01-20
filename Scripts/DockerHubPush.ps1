docker build -t mediator-mediator-chat:latest -f ./Mediator.Chat.Api/Dockerfile .
docker build -t mediator-mediator-users:latest -f ./Mediator.Users.Api/Dockerfile .
docker build -t mediator-mediator-preview:latest -f ./Mediators/Dockerfile .

docker tag mediator-mediator-chat:latest vlgaraba/mediator-chat:latest
docker tag mediator-mediator-users:latest vlgaraba/mediator-users:latest
docker tag mediator-mediator-preview:latest vlgaraba/mediator-preview:latest

docker push vlgaraba/mediator-chat:latest
docker push vlgaraba/mediator-users:latest
docker push vlgaraba/mediator-preview:latest