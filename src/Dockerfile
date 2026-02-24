# -------- Runtime image --------
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# -------- Build image --------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy all project files and restore
COPY ReadIraq.Core/ReadIraq.Core.csproj ReadIraq.Core/
COPY ReadIraq.Application/ReadIraq.Application.csproj ReadIraq.Application/
COPY ReadIraq.EntityFrameworkCore/ReadIraq.EntityFrameworkCore.csproj ReadIraq.EntityFrameworkCore/
COPY ReadIraq.Web.Core/ReadIraq.Web.Core.csproj ReadIraq.Web.Core/
COPY ReadIraq.Web.Host/ReadIraq.Web.Host.csproj ReadIraq.Web.Host/

RUN dotnet restore ReadIraq.Web.Host/ReadIraq.Web.Host.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish ReadIraq.Web.Host/ReadIraq.Web.Host.csproj -c Release -o /app/publish

# -------- Final image --------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReadIraq.Web.Host.dll"]
