FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY VPSMonitor.sln ./
COPY VPSMonitor.API/VPSMonitor.API.csproj ./VPSMonitor.API/
COPY VPSMonitor.Core/VPSMonitor.Core.csproj ./VPSMonitor.Core/

COPY VPSMonitor.API/. ./VPSMonitor.API/
COPY VPSMonitor.Core/. ./VPSMonitor.Core/
WORKDIR /src/VPSMonitor.API
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "VPSMonitor.API.dll", "--launch-profile", "https"]