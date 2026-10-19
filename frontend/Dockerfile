# Multi-stage Dockerfile para Frontend (React + Vite + TypeScript)

# Etapa 1: Compilación de la aplicación
FROM node:22-alpine AS build
WORKDIR /app

# Copiar archivos de dependencias
COPY package*.json ./

# Instalar dependencias limpias
RUN npm ci

# Copiar código fuente
COPY . .

# Argumento opcional para configurar la URL base de la API durante el build
ARG VITE_API_BASE
ENV VITE_API_BASE=${VITE_API_BASE}

# Compilar proyecto para producción
RUN npm run build

# Etapa 2: Servidor web con Nginx para servir los estáticos compilados
FROM nginx:alpine AS final

# Copiar artefactos compilados al directorio de Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copiar configuración personalizada de Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Exponer puerto HTTP estándar
EXPOSE 80

# Iniciar servidor Nginx en primer plano
CMD ["nginx", "-g", "daemon off;"]
