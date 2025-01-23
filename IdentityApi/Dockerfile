FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

COPY ./IdentityApi/IdentityApi.csproj ./IdentityApi/
COPY ./IdentityServiceApplication/IdentityServiceApplication.csproj ./IdentityServiceApplication/
COPY ./IdentityServiceDomain/IdentityServiceDomain.csproj ./IdentityServiceDomain/
COPY ./IdentityServiceInfrastructure/IdentityServiceInfrastructure.csproj ./IdentityServiceInfrastructure/

RUN dotnet restore ./IdentityApi/IdentityApi.csproj

COPY ./IdentityApi/. ./IdentityApi/
COPY ./IdentityServiceApplication/. ./IdentityServiceApplication/
COPY ./IdentityServiceDomain/. ./IdentityServiceDomain/
COPY ./IdentityServiceInfrastructure/. ./IdentityServiceInfrastructure/


RUN dotnet publish ./IdentityApi/IdentityApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IdentityApi.dll"]