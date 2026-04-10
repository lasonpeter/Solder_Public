# Build AOT binary
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends clang zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

COPY ["Solder.ServerInstanceManager/Solder.ServerInstanceManager.csproj", "Solder.ServerInstanceManager/"]
COPY ["Solder.Shared/Solder.Shared.csproj", "Solder.Shared/"]
RUN dotnet restore "Solder.ServerInstanceManager/Solder.ServerInstanceManager.csproj" -r linux-x64

COPY . .
WORKDIR "/src/Solder.ServerInstanceManager"

RUN dotnet publish "./Solder.ServerInstanceManager.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    /p:PublishAot=true \
    -o /app/publish

# Runtime with Java 8
FROM eclipse-temurin:8-jre AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libicu-dev \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish/Solder.ServerInstanceManager .

RUN mkdir -p /instance /var/solder/jars

EXPOSE 8080 25565

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV MinecraftJarService__StoragePath=/var/solder/jars
ENV ServerInstance__WorkingDirectory=/instance

ENTRYPOINT ["./Solder.ServerInstanceManager"]
