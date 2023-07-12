# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY VPSMonitor.sln ./
COPY VPSMonitor.API/VPSMonitor.API.csproj ./VPSMonitor.API/
COPY VPSMonitor.Core/VPSMonitor.Core.csproj ./VPSMonitor.Core/
# RUN dotnet restore

# Copy the remaining project files and build the application
COPY VPSMonitor.API/. ./VPSMonitor.API/
COPY VPSMonitor.Core/. ./VPSMonitor.Core/
WORKDIR /src/VPSMonitor.API
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Finalize the image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "VPSMonitor.API.dll"]