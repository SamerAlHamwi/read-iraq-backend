# Use the official ASP.NET 7 runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ReadIraq.csproj", "."]
RUN dotnet restore "ReadIraq.csproj"
COPY . .
RUN dotnet build "ReadIraq.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReadIraq.csproj" -c Release -o /app/publish

# Build the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReadIraq.dll"]