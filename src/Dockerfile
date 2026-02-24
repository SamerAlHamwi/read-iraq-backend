# -------- Runtime image --------
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# -------- Build image --------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY src/ReadIraq.csproj src/
RUN dotnet restore src/ReadIraq.csproj

COPY . .
WORKDIR /src
RUN dotnet publish ReadIraq.csproj -c Release -o /app/publish

# -------- Final image --------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ReadIraq.dll"]