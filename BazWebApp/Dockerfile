#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
# Fix Debian Buster EOL repositories and install Node.js for TypeScript compilation
RUN sed -i 's/deb.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i 's/security.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    apt-get update && apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs && rm -rf /var/lib/apt/lists/*
COPY packages /src/packages
COPY ["bazweb/BazWebApp/BazWebApp.csproj", "bazweb/BazWebApp/"]
COPY bazweb/BazWebApp bazweb/BazWebApp
WORKDIR "/src/bazweb/BazWebApp"
RUN dotnet restore "BazWebApp.csproj" --source https://https://nuget.cdn.azure.cn/v3/index.json --source /src/packages
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
