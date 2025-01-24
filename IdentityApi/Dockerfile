#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Zmieniamy œcie¿ki w zale¿noœci od lokalizacji Dockerfile
COPY ["../IdentityApi/IdentityApi.csproj", "IdentityApi/"]
COPY ["../IdentityServiceApplication/IdentityServiceApplication.csproj", "IdentityServiceApplication/"]
COPY ["../IdentityServiceDomain/IdentityServiceDomain.csproj", "IdentityServiceDomain/"]
COPY ["../IdentityServiceInfrastructure/IdentityServiceInfrastructure.csproj", "IdentityServiceInfrastructure/"]
RUN dotnet restore "IdentityApi/IdentityApi.csproj"
COPY . .
WORKDIR "/src/IdentityApi"
RUN dotnet build "IdentityApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityApi.dll"]

