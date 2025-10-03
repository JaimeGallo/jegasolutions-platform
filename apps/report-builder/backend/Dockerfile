# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/JEGASolutions.ReportBuilder.API/JEGASolutions.ReportBuilder.API.csproj", "JEGASolutions.ReportBuilder.API/"]
COPY ["src/JEGASolutions.ReportBuilder.Infrastructure/JEGASolutions.ReportBuilder.Infrastructure.csproj", "JEGASolutions.ReportBuilder.Infrastructure/"]
COPY ["src/JEGASolutions.ReportBuilder.Core/JEGASolutions.ReportBuilder.Core.csproj", "JEGASolutions.ReportBuilder.Core/"]
COPY ["src/JEGASolutions.ReportBuilder.Data/JEGASolutions.ReportBuilder.Data.csproj", "JEGASolutions.ReportBuilder.Data/"]

RUN dotnet restore "JEGASolutions.ReportBuilder.API/JEGASolutions.ReportBuilder.API.csproj"

# Copy all source files
COPY src/ .

# Build the application
WORKDIR "/src/JEGASolutions.ReportBuilder.API"
RUN dotnet build "JEGASolutions.ReportBuilder.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "JEGASolutions.ReportBuilder.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy published files
COPY --from=publish /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "JEGASolutions.ReportBuilder.API.dll"]

