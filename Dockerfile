# BASE (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Puertos
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]

RUN dotnet restore "WebApi/WebApi.csproj"
COPY . .
WORKDIR "app/WebApi"

# PUBLISH
FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

# FINAL (runtime liviano)
FROM base AS final
WORKDIR app/

COPY --from=publish /app/publish .

# Ejecutar la API
ENTRYPOINT ["dotnet", "WebApi.dll"]
