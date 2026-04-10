# Project: High-Scale Minecraft Hosting (WIP)

## Overview
A "Scale-from-Zero" hosting platform that provisions Minecraft servers dynamically to save on hardware costs.

## System Architecture

### Frontend & Management
* **Blazor WASM WebApp**: User interface for ServerCreation and instance management.
* **.NET ContainerManager**: Backend service using Docker SDK to spawn and label containers.

### Routing & Networking
* **mc-router**: Minecraft gateway; wakes up stopped containers when players join.
* **Traefik v3**: Web gateway; handles SSL and SignalR console traffic.
* **Internal Network**: Single bridge network (172.20.0.0/16) for private container communication.

## Lifecycle Workflow

1. **Provision**: WebApp/Manager creates a stopped container with specific metadata labels.
2. **Idle**: mc-router monitors the container but it consumes zero RAM/CPU.
3. **Wake**: Player connects -> mc-router starts container -> Traefik routes web console.
4. **Sleep**: 5 minutes of inactivity -> mc-router stops container to reclaim resources.

## Core Configuration (Labels)

| Target | Key | Value/Logic |
| :--- | :--- | :--- |
| **mc-router** | mc-router.host | {serverId}.game.lasonpeter.net |
| **mc-router** | mc-router.auto-scale-up | true |
| **Traefik** | traefik.enable | true |
| **Traefik** | traefik.http.routers.rule | Host({serverId}-console.lasonpeter.net) |

## Current Status / Best Practices
* **Isolation**: Use NanoCPUs/Memory limits to prevent noisy neighbors.
* **Persistence**: SignalR hubs require heartbeats and sticky sessions for stable consoles.
* **Security**: Only ports 443 and 25565 are exposed to the host.
