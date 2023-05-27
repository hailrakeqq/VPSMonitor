FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dotnet-build
WORKDIR /src
COPY . /src
RUN dotnet restore "VPSMonitor.API.csproj"
RUN dotnet build "VPSMonitor.API.csproj" -c Release -o /app/build

FROM dotnet-build AS dotnet-publish
RUN dotnet publish "VPSMonitor.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=dotnet-publish /app/publish .

ENTRYPOINT ["dotnet", "VPSMonitor.API.dll"]