# Use the official ASP.NET runtime image as the base image for the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5030

# Use the .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the csproj files to restore dependencies before copying the full source
COPY ["PeopleApi/PeopleApi.csproj", "PeopleApi/"]
COPY ["PeopleServiceApplication/PeopleServiceApplication.csproj", "PeopleServiceApplication/"]
COPY ["PeopleServiceDomain/PeopleServiceDomain.csproj", "PeopleServiceDomain/"]
COPY ["PeopleServiceInfrastructure/PeopleServiceInfrastructure.csproj", "PeopleServiceInfrastructure/"]

# Restore the dependencies
RUN dotnet restore "PeopleApi/PeopleApi.csproj"

# Copy the rest of the application files
COPY . .

# Build the application in Release configuration
WORKDIR "/src/PeopleApi"
RUN dotnet build "PeopleApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PeopleApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the base image to run the app and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point to run the app
ENTRYPOINT ["sh", "-c", "dotnet PeopleApi.dll"]
