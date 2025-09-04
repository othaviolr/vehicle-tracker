# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY VehicleTracker.sln .

# Copy project files
COPY VehicleTracker.Domain/VehicleTracker.Domain.csproj VehicleTracker.Domain/
COPY VehicleTracker.Application/VehicleTracker.Application.csproj VehicleTracker.Application/
COPY VehicleTracker.Infrastructure/VehicleTracker.Infrastructure.csproj VehicleTracker.Infrastructure/
COPY VehicleTracker.API/VehicleTracker.API.csproj VehicleTracker.API/

# Clear any cached NuGet data and restore dependencies
RUN dotnet nuget locals all --clear
RUN dotnet restore --no-cache

# Copy source code
COPY . .

# Build and publish (com restore para garantir)
WORKDIR /src/VehicleTracker.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "VehicleTracker.API.dll"]