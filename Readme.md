### A playground project for MassTransit

Including:
- MT Transactional Outbox with EF Core.
- Rabbit MQ.
- Async Integration Event processing.
- Minimal APIs

### Questions

- Struggling to understand how, with MassTransit, I can have more fine-grained control over the message Publishing. For example:
  -	If I have turned on the Outbox then all Published messages go via SQL Outbox before being dispatched - but what if I want to dispatch an in-memory, in-process Domain Event to execute BEFORE the Save (as part of the Save Transaction)
	- Does this mean "true" Domain Events to update the Domain Model need to be done some other way?
  - If I have turned on Rabbit MQ, then all Published messages will be dispatched via RabbitMQ - but what if I want to dispatch an in-memory, in-process Domain Event?
	- Do I need to use MultiBus and configure a local one? https://masstransit.io/documentation/configuration/multibus
	- But ideally Publisher shouldn't need to know this...

### Future?

- Domain Events vs Integration - updating Domain before SaveAsync
- Clarity over async processing within same Bounded Context vs separate Bounded Context / Microservice.


# Setup - Non-Docker

- Ensure you have SQL LocalDb installed (I think it is part of VS installation)
- Install chocolately https://chocolatey.org/install
- Install RabbitMq: `choco install rabbitmq` (see https://www.rabbitmq.com/install-windows.html#chocolatey)
   - OR start docker container: `docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq` which has the GUI enabled on http://localhost:15672/ (guest, guest)

# Setup - Docker

**_First, ensure you are in the root folder of the repo._**

Note: In all cases `-d` runs as a daemon, so remove if you want to run interactively.

## Build

```[pwsh]
docker build -t masstransitplay_issuesapi -f .\IssuesApi-Microservice\MassTransitPlay.IssuesApi\Dockerfile .
docker build -t masstransitplay_issuesapi_worker -f .\IssuesApi-Microservice\MassTransitPlay.IssuesApi.Worker\Dockerfile .
docker build -t masstransitplay_issuesstats -f .\IssuesStats-Microservice\MassTransitPlay.Stats\Dockerfile .
```

## Dependencies

#### Create User-Defined Bridge Network

This allows the containers to be on the same bridge network and therefore communicate with hostnames instead of IPs.

```[pwsh]
docker network create masstransit-play-net
```

#### Start SQL

```[pwsh]
docker run -d --net masstransit-play-net --hostname issuesdockersql --name issuessql -p 1433:1433 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Docker-sql-pass" mcr.microsoft.com/mssql/server:2022-latest
```

#### Start RabbitMQ

```[pwsh]
docker run -d --net masstransit-play-net --hostname issuesrabbitmq --name issuesrabbitmq -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

## Run

```[pwsh]
docker run -d -it --rm --net masstransit-play-net --name MassTransitPlay.IssuesApi -p 7246:443 -p 5160:80 -e ASPNETCORE_ENVIRONMENT=Development -e ConnectionStrings:IssueTrackerContext="Server=issuesdockersql,1433;Database=IssueTrackerMassTransit;User Id=sa;Password=Docker-sql-pass;MultipleActiveResultSets=true;TrustServerCertificate=True" -e ConnectionStrings:RabbitMq="amqp://issuesrabbitmq:5672" masstransitplay_issuesapi
docker run -d -it --rm --net masstransit-play-net --name MassTransitPlay.IssuesApi.Worker -e DOTNET_ENVIRONMENT=Development -e ConnectionStrings:IssueTrackerContext="Server=issuesdockersql,1433;Database=IssueTrackerMassTransit;User Id=sa;Password=Docker-sql-pass;MultipleActiveResultSets=true;TrustServerCertificate=True" -e ConnectionStrings:RabbitMq="amqp://issuesrabbitmq:5672" masstransitplay_issuesapi_worker
docker run -d -it --rm --net masstransit-play-net --name MassTransitPlay.Stats -e DOTNET_ENVIRONMENT=Development -e ConnectionStrings:IssueStatsContext="Server=issuesdockersql,1433;Database=IssueStatsMassTransit;User Id=sa;Password=Docker-sql-pass;MultipleActiveResultSets=true;TrustServerCertificate=True" -e ConnectionStrings:RabbitMq="amqp://issuesrabbitmq:5672" masstransitplay_issuesstats
```

## Use

Visit [http://localhost:5160/swagger/](http://localhost:5160/swagger/) - not sure why https not working yet; it seems that the project starts up on port 80 only.

# Setup - Docker Compose

```[pwsh]
docker compose up
```