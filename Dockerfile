# first-stage base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# copy and publish application files
COPY . .
RUN dotnet publish -c release -o /app

# final stage image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT [ "dotnet", "API.dll"]

