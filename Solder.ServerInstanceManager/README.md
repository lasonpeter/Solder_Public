# Solder ServerInstanceManager - Java & Minecraft JAR Management

This service provides automatic Java version management and Minecraft server JAR downloads for hosting Minecraft servers
across different versions.

## Architecture

**IMPORTANT**: Each Docker container runs **BOTH** the .NET management API and the Minecraft server process in the *
*SAME CONTAINER**.

```
┌─────────────────────────────────────┐
│   Container (Instance 1)            │
│  ┌────────────────────────────┐     │
│  │ .NET ServerInstanceManager │     │  Port 8080 (API)
│  │  - Manages server lifecycle│────►│
│  │  - Downloads JARs          │     │
│  │  - Reports to WebAPI       │     │
│  └────────────────────────────┘     │
│  ┌────────────────────────────┐     │
│  │ Minecraft Server Process   │     │  Port 25565 (Game)
│  │  - Runs in same container  │────►│
│  │  - Managed by .NET app     │     │
│  └────────────────────────────┘     │
│   SERVER_INSTANCE_ID: <unique-guid> │
└─────────────────────────────────────┘
```

**Key Points:**

- ✅ One container = One server instance
- ✅ Each container has a unique `SERVER_INSTANCE_ID` (GUID)
- ✅ .NET app and Minecraft server run in same container
- ✅ Shared JAR volume across containers (saves bandwidth)
- ✅ Per-instance data volume for world saves

## Features

- **Automatic Java Version Detection**: Auto-detects required Java version based on Minecraft version
- **Multi-Version Support**: Supports Java 8, 17, and 21 through Docker multi-stage builds
- **Automatic JAR Downloads**: Downloads server JARs from official sources (Mojang, PaperMC)
- **Intelligent Caching**: Stores downloaded JARs locally to avoid re-downloading
- **Version Management**: Supports Vanilla, Paper, and Spigot server types
- **Instance Isolation**: Each container is a complete, isolated server instance

## Quick Start

### Using Docker

Build with specific Java version:

```bash
# Java 17 (default, recommended for Minecraft 1.18+)
docker build --build-arg JAVA_VERSION=17 -t solder-instance:java17 .

# Java 21 (for Minecraft 1.20.5+)
docker build --build-arg JAVA_VERSION=21 -t solder-instance:java21 .

# Java 8 (for legacy Minecraft 1.16.5 and below)
docker build --build-arg JAVA_VERSION=8 -t solder-instance:java8 .
```

Build the current MVP image used by compose:

```bash
./Solder.ServerInstanceManager/build-image.sh
# optional custom tag
./Solder.ServerInstanceManager/build-image.sh my-registry/solder-sim:java8-aot
```

Run a single instance (MUST provide SERVER_INSTANCE_ID):

```bash
# Generate a unique GUID for this instance
INSTANCE_ID=$(uuidgen)

docker run -d \
  --name mc-instance-$INSTANCE_ID \
  -p 8080:8080 \
  -p 25565:25565 \
  -v minecraft-jars:/var/solder/jars \
  -v instance-data-$INSTANCE_ID:/instance \
  -e SERVER_INSTANCE_ID=$INSTANCE_ID \
  -e ServerInstance__DefaultMinecraftVersion=1.20.1 \
  -e ServerInstance__DefaultServerType=vanilla \
  solder-instance:java17
```

### Using Docker Compose (Recommended)

```bash
# Start all example instances
docker-compose up -d

# Start specific instance
docker-compose up -d mc-instance-1

# View logs for specific instance
docker-compose logs -f mc-instance-1
```

## Configuration

### appsettings.json

```json
{
  "MinecraftJarService": {
    "StoragePath": "/var/solder/jars",
    "CleanupIntervalDays": 30,
    "DownloadTimeoutSeconds": 300,
    "EnableAutoCleanup": true
  },
  "ServerInstance": {
    "WorkingDirectory": "/var/solder/instances",
    "DefaultXmx": "1024",
    "DefaultXms": "1024",
    "DefaultServerType": "vanilla",
    "DefaultMinecraftVersion": "1.20.1"
  }
}
```

### instance_config.json (per-instance)

Create this file in your working directory to configure a specific server instance:

```json
{
  "MinecraftVersion": "1.20.1",
  "ServerType": "vanilla",
  "JavaVersion": "17",
  "Xmx": "2048",
  "Xms": "1024",
  "working_directory": "/var/solder/instances/my-server"
}
```

## Java Version Requirements

| Minecraft Version | Required Java | Auto-detected |
|-------------------|---------------|---------------|
| 1.16.5 and below  | Java 8        | ✅             |
| 1.17.x            | Java 17       | ✅             |
| 1.18.x - 1.20.4   | Java 17       | ✅             |
| 1.20.5+           | Java 21       | ✅             |

## Supported Server Types

### Vanilla

- Source: Official Mojang API
- Versions: All official releases
- Auto-download: ✅

### Paper

- Source: PaperMC API
- Versions: Latest builds for each version
- Auto-download: ✅

### Spigot

- Source: Requires BuildTools
- Auto-download: ❌ (Use Paper as alternative)

## API Endpoints

### Get Available Versions

```bash
GET /api/minecraft/versions?type=vanilla
```

Response:

```json
[
  {
    "version": "1.20.1",
    "type": "Vanilla",
    "releasedAt": "2023-06-12T00:00:00Z",
    "javaVersion": "17"
  }
]
```

### Start Server Instance

```bash
POST /api/instance/start
```

The service will automatically:

1. Check if JAR exists locally
2. Download if missing from official sources
3. Validate file integrity
4. Detect required Java version
5. Start the server process

## Directory Structure (Inside Container)

```
Container 1 (Instance ID: 019d7307...)
/app/                           # .NET application
/instance/                      # THIS instance's data (volume mounted)
  ├── world/                    # Minecraft world data
  ├── server.properties         # Server config
  └── instance_config.json      # Solder config
/var/solder/jars/               # Shared JARs (volume mounted)
  ├── 1.20.1/
  │   ├── vanilla/
  │   │   └── server-1.20.1.jar
  │   └── paper/
  │       └── server-1.20.1.jar
  └── 1.16.5/
      └── vanilla/
          └── server-1.16.5.jar

Container 2 (Instance ID: 2a1b3c4d...)
/app/                           # .NET application
/instance/                      # DIFFERENT instance data (different volume)
  ├── world/
  ├── server.properties
  └── instance_config.json
/var/solder/jars/               # SAME shared JARs (same volume)
  └── (shared with Container 1)
```

## Environment Variables

| Variable                                  | Description                                 | Required  | Default            |
|-------------------------------------------|---------------------------------------------|-----------|--------------------|
| **`SERVER_INSTANCE_ID`**                  | **Unique GUID for this instance**           | **✅ YES** | **None**           |
| `JAVA_VERSION`                            | Java major version in container (build-arg) | No        | `17`               |
| `MinecraftJarService__StoragePath`        | JAR storage location                        | No        | `/var/solder/jars` |
| `ServerInstance__WorkingDirectory`        | Instance working directory                  | No        | `/instance`        |
| `ServerInstance__DefaultMinecraftVersion` | Default MC version                          | No        | `1.20.1`           |
| `ServerInstance__DefaultServerType`       | Default server type                         | No        | `vanilla`          |

**⚠️ CRITICAL**: `SERVER_INSTANCE_ID` is **REQUIRED**. Each container MUST have a unique GUID passed as an environment
variable.

## Troubleshooting

### JAR Download Fails

- Check internet connectivity
- Verify Mojang/PaperMC APIs are accessible
- Check logs for specific error messages

### Wrong Java Version

- Ensure correct Docker image built with appropriate `JAVA_VERSION`
- Check `instance_config.json` for manual Java version override
- Verify Minecraft version compatibility

### Out of Memory

- Increase `Xmx` and `Xms` values in configuration
- Adjust Docker container memory limits

## Development

### Build from Source

```bash
dotnet build Solder.ServerInstanceManager.csproj
dotnet run
```

### Run Tests

```bash
dotnet test ../Solder.Testing/
```

## License

See main project LICENSE file.
