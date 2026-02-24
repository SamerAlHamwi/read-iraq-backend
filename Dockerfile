# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ReadIraq.sln .
COPY src/ReadIraq.Core/ReadIraq.Core.csproj src/ReadIraq.Core/
COPY src/ReadIraq.Application/ReadIraq.Application.csproj src/ReadIraq.Application/
COPY src/ReadIraq.EntityFrameworkCore/ReadIraq.EntityFrameworkCore.csproj src/ReadIraq.EntityFrameworkCore/
COPY src/ReadIraq.Web.Core/ReadIraq.Web.Core.csproj src/ReadIraq.Web.Core/
COPY src/ReadIraq.Web.Host/ReadIraq.Web.Host.csproj src/ReadIraq.Web.Host/
COPY src/ReadIraq.Migrator/ReadIraq.Migrator.csproj src/ReadIraq.Migrator/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY src/ src/

# Build and Publish API
RUN dotnet publish src/ReadIraq.Web.Host/ReadIraq.Web.Host.csproj -c Release -o /app/publish/api

# Build and Publish Migrator
RUN dotnet publish src/ReadIraq.Migrator/ReadIraq.Migrator.csproj -c Release -o /app/publish/migrator

# Runtime Stage - API
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app
COPY --from=build /app/publish/api .
EXPOSE 80
ENTRYPOINT ["dotnet", "ReadIraq.Web.Host.dll"]

# Runtime Stage - Migrator (used via docker-compose override or command)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS migrator
WORKDIR /app
COPY --from=build /app/publish/migrator .
ENTRYPOINT ["dotnet", "ReadIraq.Migrator.dll"]
