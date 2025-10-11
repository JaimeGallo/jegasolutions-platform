# ====================================
# OPTIMIZED DOCKERFILE FOR EXTRA HOURS
# ====================================

# Runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ONLY project files first (mejor caché)
COPY ["src/JEGASolutions.ExtraHours.API/JEGASolutions.ExtraHours.API.csproj", "JEGASolutions.ExtraHours.API/"]
COPY ["src/JEGASolutions.ExtraHours.Core/JEGASolutions.ExtraHours.Core.csproj", "JEGASolutions.ExtraHours.Core/"]
COPY ["src/JEGASolutions.ExtraHours.Data/JEGASolutions.ExtraHours.Data.csproj", "JEGASolutions.ExtraHours.Data/"]
COPY ["src/JEGASolutions.ExtraHours.Infrastructure/JEGASolutions.ExtraHours.Infrastructure.csproj", "JEGASolutions.ExtraHours.Infrastructure/"]

# Restore dependencies (esta capa se cachea si no cambian los .csproj)
RUN dotnet restore "JEGASolutions.ExtraHours.API/JEGASolutions.ExtraHours.API.csproj"

# Ahora sí, copiar todo el código fuente
COPY src/ .

# Build y Publish en un solo paso (más rápido)
WORKDIR "/src/JEGASolutions.ExtraHours.API"
RUN dotnet publish "JEGASolutions.ExtraHours.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Final runtime stage (imagen mínima)
FROM base AS final
WORKDIR /app

# Copiar binarios publicados
COPY --from=build /app/publish .

# Configuración de entorno
ENV ASPNETCORE_URLS=http://+:80

# Punto de entrada
ENTRYPOINT ["dotnet", "JEGASolutions.ExtraHours.API.dll"]
