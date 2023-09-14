#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IssuesApi-Microservice/MassTransitPlay.IssuesApi/MassTransitPlay.IssuesApi.csproj", "IssuesApi-Microservice/MassTransitPlay.IssuesApi/"]
COPY ["IssuesApi-Microservice/MassTransitPlay.IssuesApi.Domain/MassTransitPlay.IssuesApi.Domain.csproj", "IssuesApi-Microservice/MassTransitPlay.IssuesApi.Domain/"]
RUN dotnet restore "IssuesApi-Microservice/MassTransitPlay.IssuesApi/MassTransitPlay.IssuesApi.csproj"
COPY . .
WORKDIR "/src/IssuesApi-Microservice/MassTransitPlay.IssuesApi"
RUN dotnet build "MassTransitPlay.IssuesApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitPlay.IssuesApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitPlay.IssuesApi.dll"]