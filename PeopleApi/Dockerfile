#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../PeopleApi/PeopleApi.csproj", "PeopleApi/"]
COPY ["../PeopleServiceApplication/PeopleServiceApplication.csproj", "PeopleServiceApplication/"]
COPY ["../PeopleServiceDomain/PeopleServiceDomain.csproj", "PeopleServiceDomain/"]
COPY ["../PeopleServiceInfrastructure/PeopleServiceInfrastructure.csproj", "PeopleServiceInfrastructure/"]
RUN dotnet restore "PeopleApi/PeopleApi.csproj"
COPY . .
WORKDIR "/src/PeopleApi"
RUN dotnet build "PeopleApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PeopleApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeopleApi.dll"]