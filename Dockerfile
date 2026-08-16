FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY PortFolioAPI/PortFolioAPI.csproj PortFolioAPI/
COPY DataAccess/DataAccess.csproj DataAccess/

RUN dotnet restore PortFolioAPI/PortFolioAPI.csproj

COPY PortFolioAPI/ PortFolioAPI/
COPY DataAccess/ DataAccess/

WORKDIR /src/PortFolioAPI

RUN dotnet publish PortFolioAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PortFolioAPI.dll"]