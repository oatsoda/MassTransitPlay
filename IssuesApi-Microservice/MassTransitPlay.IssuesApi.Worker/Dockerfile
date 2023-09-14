#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IssuesApi-Microservice/MassTransitPlay.IssuesApi.Worker/MassTransitPlay.IssuesApi.Worker.csproj", "IssuesApi-Microservice/MassTransitPlay.IssuesApi.Worker/"]
COPY ["IssuesApi-Microservice/MassTransitPlay.IssuesApi.Domain/MassTransitPlay.IssuesApi.Domain.csproj", "IssuesApi-Microservice/MassTransitPlay.IssuesApi.Domain/"]
RUN dotnet restore "IssuesApi-Microservice/MassTransitPlay.IssuesApi.Worker/MassTransitPlay.IssuesApi.Worker.csproj"
COPY . .
WORKDIR "/src/IssuesApi-Microservice/MassTransitPlay.IssuesApi.Worker"
RUN dotnet build "MassTransitPlay.IssuesApi.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitPlay.IssuesApi.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitPlay.IssuesApi.Worker.dll"]