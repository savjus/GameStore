# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY game-store-v2-with-branches.sln ./
COPY Directory.Build.props ./
COPY GameStore/GameStore.csproj GameStore/
COPY GameStore.Tests/GameStore.Tests.csproj GameStore.Tests/

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

# ---- NEW: Angular build stage ----
FROM node:20-alpine AS uibuilder
WORKDIR /ui
COPY Gamestore-ui/package*.json ./
RUN npm ci
COPY Gamestore-ui/ ./
RUN npm run build -- --configuration production

# ---- NEW: nginx stage to serve the UI ----
FROM nginx:alpine AS uiserver
COPY --from=uibuilder /ui/dist/Gamestore-ui/browser /usr/share/nginx/html
RUN printf 'server {\n\
    listen 80;\n\
    root /usr/share/nginx/html;\n\
    index index.html;\n\
    location / {\n\
        try_files $uri $uri/ /index.html;\n\
    }\n\
    location /api/ {\n\
        proxy_pass http://api:8080/;\n\
        proxy_http_version 1.1;\n\
        proxy_set_header Host $host;\n\
        proxy_set_header X-Real-IP $remote_addr;\n\
    }\n\
}\n' > /etc/nginx/conf.d/default.conf
EXPOSE 80