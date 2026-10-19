# Sistema de Gestión de Fútbol Corporativo

Sistema integral para la gestión de torneos corporativos de fútbol, desarrollado con arquitectura limpia (Clean Architecture), CQRS (.NET 9 + MediatR + EF Core + Dapper) y frontend en React (TypeScript + Vite).

---

## 🐳 Despliegue con Docker

El proyecto incluye soporte completo para contenedores Docker y orquestación con Docker Compose.

### Requisitos previos
- [Docker](https://docs.docker.com/get-docker/) (versión 20.10 o superior)
- [Docker Compose](https://docs.docker.com/compose/) (versión v2+)

---

### Levantar todo el stack con Docker Compose (Recomendado)

Docker Compose iniciará la base de datos PostgreSQL, la API Backend (.NET 9) y la aplicación Frontend (Nginx + React) conectados en una red interna privada:

```bash
docker compose up --build -d
```

#### Servicios y accesos:

| Servicio | Contenedor | Puerto Local | Descripción |
|---|---|---|---|
| **Frontend** | `futbol_frontend` | [http://localhost:3000](http://localhost:3000) | Aplicación Web React servida por Nginx |
| **Backend API** | `futbol_backend` | [http://localhost:5193/swagger](http://localhost:5193/swagger) | API RESTful y documentación Swagger |
| **PostgreSQL** | `futbol_postgres` | `localhost:5433` | Base de datos PostgreSQL (puerto 5433 para evitar conflictos) |

Para detener todos los servicios:
```bash
docker compose down
```

Para ver los logs en tiempo real:
```bash
docker compose logs -f
```

---

### Compilar y ejecutar contenedores individualmente

#### 1. Backend (.NET 9 Web API)

Compilar la imagen:
```bash
docker build -t futbol-backend -f src/SistemaGestionFutbolCorporativo.Api/Dockerfile .
```

Ejecutar el contenedor:
```bash
docker run -d \
  --name futbol_backend \
  -p 5193:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__Database="Host=host.docker.internal;Port=5432;Username=admin;Password=789520;Database=SistemaGestionFutbolCorporativo" \
  futbol-backend
```

#### 2. Frontend (React + Vite + Nginx)

Compilar la imagen:
```bash
docker build -t futbol-frontend -f frontend/Dockerfile ./frontend
```

Ejecutar el contenedor:
```bash
docker run -d \
  --name futbol_frontend \
  -p 3000:80 \
  futbol-frontend
```

---

## 🛠️ Estructura de archivos Docker

- [`src/SistemaGestionFutbolCorporativo.Api/Dockerfile`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/src/SistemaGestionFutbolCorporativo.Api/Dockerfile): Multi-stage build para la API (.NET SDK 9.0 -> ASP.NET Runtime 9.0).
- [`frontend/Dockerfile`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/frontend/Dockerfile): Multi-stage build para el Frontend (Node.js 22 Alpine -> Nginx Alpine).
- [`frontend/nginx.conf`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/frontend/nginx.conf): Servidor Nginx con compresión gzip, caché de estáticos, soporte de SPA y proxy inverso a `/api/`.
- [`docker-compose.yml`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/docker-compose.yml): Orquestador de PostgreSQL, Backend y Frontend con healthchecks y variables configurables.
- [`.dockerignore`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/.dockerignore) y [`frontend/.dockerignore`](file:///Users/mcastro/Projects/SistemaGestionFutbolCorporativo/frontend/.dockerignore): Exclusiones de artefactos locales (`bin/`, `obj/`, `node_modules/`, etc.).
