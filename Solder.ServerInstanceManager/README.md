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

