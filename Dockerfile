# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY game-store-v2-with-branches.sln ./
COPY Directory.Build.props ./
COPY GameStore/GameStore.csproj GameStore/
COPY GameStore.Tests/GameStore.Tests.csproj GameStore.Tests/
COPY dashboard/dashboard.ServiceDefaults/dashboard.ServiceDefaults.csproj dashboard/dashboard.ServiceDefaults/

RUN dotnet restore

COPY . .
WORKDIR /src/GameStore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GameStore.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner
WORKDIR /src
COPY . .
ENTRYPOINT ["dotnet", "test", "GameStore.Tests/GameStore.Tests.csproj", "-c", "Release"]
