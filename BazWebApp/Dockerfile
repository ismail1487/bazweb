#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs
COPY ["BazWebApp/BazWebApp.csproj", "BazWebApp/"]
RUN dotnet restore "BazWebApp/BazWebApp.csproj" -s http://172.16.31.10/NugetServer/nuget -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/BazWebApp"
RUN npm install
RUN dotnet build "BazWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BazWebApp.csproj" -c Release -o /app/publish

FROM base AS final

# Change timezone to local time
ENV TZ=Europe/Istanbul
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BazWebApp.dll"]