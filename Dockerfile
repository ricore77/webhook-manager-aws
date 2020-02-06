FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copiar csproj e restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Build da aplicacao
COPY /. ./
RUN dotnet publish -c Release -o out

# Build da imagem
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "WebhookManager.Api\WebhookManager.Api.csproj"]
