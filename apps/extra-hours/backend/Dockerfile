# Backend Dockerfile for .NET 8 API (HTTP only for development)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExtraHours.API/ExtraHours.API.csproj", "ExtraHours.API/"]
RUN dotnet restore "ExtraHours.API/ExtraHours.API.csproj"
COPY . .
WORKDIR "/src/ExtraHours.API"
RUN dotnet publish "ExtraHours.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ExtraHours.API.dll"]
