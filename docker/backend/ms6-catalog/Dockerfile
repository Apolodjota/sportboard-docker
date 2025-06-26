# Etapa 1: Construcción del proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar archivo de solución y dependencias
COPY backend/Catalogs/*.csproj ./Catalogs/
RUN dotnet restore ./Catalogs/*.csproj

# Copiar el código fuente y compilar
COPY backend/Catalogs/ ./Catalogs/
WORKDIR /source/Catalogs
RUN dotnet publish -c Release -o /app

# Copiar archivos de datos
COPY backend/Catalogs/SeedData/backup_groups.csv /app/SeedData/
COPY backend/Catalogs/SeedData/backup_catalogs.csv /app/SeedData/

# Etapa 2: Ejecutar el proyecto
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar archivos desde la etapa de construcción
COPY --from=build /app .

# Exponer el puerto por defecto
EXPOSE 5241
ENV ASPNETCORE_URLS=http://localhost:5241

# Comando de inicio
ENTRYPOINT ["dotnet", "Catalogs.dll"]
