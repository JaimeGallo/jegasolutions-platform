# Backend Dockerfile for .NET 8 API (HTTP only for development)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/JEGASolutions.ExtraHours.API/JEGASolutions.ExtraHours.API.csproj", "JEGASolutions.ExtraHours.API/"]
COPY ["src/JEGASolutions.ExtraHours.Core/JEGASolutions.ExtraHours.Core.csproj", "JEGASolutions.ExtraHours.Core/"]
COPY ["src/JEGASolutions.ExtraHours.Data/JEGASolutions.ExtraHours.Data.csproj", "JEGASolutions.ExtraHours.Data/"]
COPY ["src/JEGASolutions.ExtraHours.Infrastructure/JEGASolutions.ExtraHours.Infrastructure.csproj", "JEGASolutions.ExtraHours.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "JEGASolutions.ExtraHours.API/JEGASolutions.ExtraHours.API.csproj"

# Copy everything else
COPY src/ .

# Build and publish
WORKDIR "/src/JEGASolutions.ExtraHours.API"
RUN dotnet publish "JEGASolutions.ExtraHours.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "JEGASolutions.ExtraHours.API.dll"]
