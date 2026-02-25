# ========================
# BUILD STAGE
# ========================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy full solution
COPY . .

# Go to Web Host project
WORKDIR /src/src/ReadIraq.Web.Host

# Restore & publish
RUN dotnet restore ReadIraq.Web.Host.csproj
RUN dotnet publish ReadIraq.Web.Host.csproj -c Release -o /app/publish

# ========================
# RUNTIME STAGE
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8086

ENV ASPNETCORE_URLS=http://+:8086
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ReadIraq.Web.Host.dll"]