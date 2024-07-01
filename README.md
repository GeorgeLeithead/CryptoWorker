# CryptoWorker
A worker project for the retrieval and storage of Crypto details.

# DOTNET project commands

dotnet publish --os linue /t:PublishContainer


# DOCKER commands

## Login
To log in to a remote/local docker:
docker login -u casaos

## Images

### Listing
docker image ls


## Containers

### Listing
To list containers:
docker container ls

To list containers, including legacy (not imported to CASAOS)
docker container ls --all

## Logs
To get the last 5 log entries from a container:
docker logs -n 5 CONTAINER

## RUN
To run/create a container from an image
docker run -d IMAGEIDorIMAGENAME