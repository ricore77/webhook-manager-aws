FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

#EXPOSE 5000

# Copiar csproj e restaurar dependencias
COPY /WebhookManager/*.csproj ./
RUN dotnet restore

# Build da aplicacao
COPY /WebhookManager/. ./
RUN dotnet publish -c Release -o out

# Build da imagem
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "WebhookManager.Api.dll"]
