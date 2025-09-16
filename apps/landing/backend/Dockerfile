# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Imagen para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto (.csproj solamente)
COPY ["src/JEGASolutions.Landing.API/JEGASolutions.Landing.API.csproj", "src/JEGASolutions.Landing.API/"]
COPY ["src/JEGASolutions.Landing.Application/JEGASolutions.Landing.Application.csproj", "src/JEGASolutions.Landing.Application/"]
COPY ["src/JEGASolutions.Landing.Domain/JEGASolutions.Landing.Domain.csproj", "src/JEGASolutions.Landing.Domain/"]
COPY ["src/JEGASolutions.Landing.Infrastructure/JEGASolutions.Landing.Infrastructure.csproj", "src/JEGASolutions.Landing.Infrastructure/"]

# Restaurar dependencias del proyecto principal
RUN dotnet restore "src/JEGASolutions.Landing.API/JEGASolutions.Landing.API.csproj"

# Copiar todo el código fuente
COPY . .

# Build del proyecto principal (no la solución completa)
RUN dotnet build "src/JEGASolutions.Landing.API/JEGASolutions.Landing.API.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "src/JEGASolutions.Landing.API/JEGASolutions.Landing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JEGASolutions.Landing.API.dll"]