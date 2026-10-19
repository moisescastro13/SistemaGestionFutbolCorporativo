# Multi-stage Dockerfile para Backend (.NET 9 Web API)

# Etapa 1: Compilación y publicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos para aprovechar el cache de dependencias
COPY ["SistemaGestionFutbolCorporativo.sln", "./"]
COPY ["src/SistemaGestionFutbolCorporativo.Api/SistemaGestionFutbolCorporativo.Api.csproj", "src/SistemaGestionFutbolCorporativo.Api/"]
COPY ["src/SistemaGestionFutbolCorporativo.Application/SistemaGestionFutbolCorporativo.Application.csproj", "src/SistemaGestionFutbolCorporativo.Application/"]
COPY ["src/SistemaGestionFutbolCorporativo.Domain/SistemaGestionFutbolCorporativo.Domain.csproj", "src/SistemaGestionFutbolCorporativo.Domain/"]
COPY ["src/SistemaGestionFutbolCorporativo.Infrastructure/SistemaGestionFutbolCorporativo.Infrastructure.csproj", "src/SistemaGestionFutbolCorporativo.Infrastructure/"]

RUN dotnet restore "SistemaGestionFutbolCorporativo.sln"

COPY ["src/", "src/"]
WORKDIR "/src/src/SistemaGestionFutbolCorporativo.Api"
RUN dotnet publish "SistemaGestionFutbolCorporativo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SistemaGestionFutbolCorporativo.Api.dll"]
