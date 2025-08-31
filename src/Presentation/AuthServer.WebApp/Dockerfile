# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
ARG BUILD_CONFIGURATION=Release

# Copy csproj files
COPY src/Domain/AuthServer.Domain/*.csproj Domain/AuthServer.Domain/
COPY src/Application/AuthServer.Application/*.csproj Application/AuthServer.Application/
COPY src/Infrastructure/AuthServer.Infrastructure/*.csproj Infrastructure/AuthServer.Infrastructure/
COPY src/Presentation/AuthServer.WebApp/*.csproj Presentation/AuthServer.WebApp/

# Restore dependencies
RUN dotnet restore "Presentation/AuthServer.WebApp/AuthServer.WebApp.csproj"

# Copy everything else
COPY src/ .

WORKDIR /src/Presentation/AuthServer.WebApp
RUN dotnet build "AuthServer.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthServer.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServer.WebApp.dll"]
