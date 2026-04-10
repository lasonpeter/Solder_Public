High-Scale Minecraft Hosting Architecture
This guide outlines the integration of mc-router, Traefik v3, and a custom .**NET** ContainerManager to support hundreds
of dynamically allocated Minecraft server instances with custom wrappers.

## Network Strategy

Use a single, large internal Docker bridge network to allow the infrastructure containers to communicate with
dynamically spawned backends.
Network Name: mc-network
Subnet: **172**.20.0.0/16 (Provides ~65,**000** internal IPs).
Security: Only Traefik (**443**) and mc-router (**25565**) are exposed to the host. Backend containers are private to
the bridge.

## Infrastructure Configuration

services:
traefik:
image: traefik:v3.0
container_name: traefik
command:

- *--providers.docker=true*
- *--providers.docker.exposedbydefault=false*
- *--entrypoints.websecure.address=:**443***
- "--certificatesresolvers.myresolver.acme.dnschallenge=true*
- *--certificatesresolvers.myresolver.acme.dnschallenge.provider=route53*
- *--certificatesresolvers.myresolver.acme.email=[contact@lasonpeter.net](mailto:contact@lasonpeter.net)*
- *--certificatesresolvers.myresolver.acme.storage=/letsencrypt/acme.json*
  ports:
- ***443**:**443**"
  environment:
- AWS_ACCESS_KEY_ID=${AWS_ACCESS_KEY_ID}
- AWS_SECRET_ACCESS_KEY=${AWS_SECRET_ACCESS_KEY}
- AWS_REGION=eu-central-1
  volumes:
- /var/run/docker.sock:/var/run/docker.sock:ro
- ./letsencrypt:/letsencrypt
  networks:
- mc-network

  mc-router:
  image: itzg/mc-router
  container_name: mc-router
  command:
    - *--in-docker*
    - *--docker-refresh-interval=30s*
    - *--auto-scale-up*
    - *--auto-scale-down*
    - *--auto-scale-down-after=5m*
    - "--auto-scale-asleep-motd=§cServer is Sleeping... §7Join to Wake It Up!*
      ports:
    - ***25565**:**25565**"
      volumes:
    - /var/run/docker.sock:/var/run/docker.sock:ro
      networks:
    - mc-network

## ContainerManager (Implementation Logic)

Your manager should use the Docker **SDK** to spawn containers with these specific labels. Use the unique Server ID to
generate subdomains. ### Required Labels Category ### Label Key Logic / Value mc-router mc-router.host
{serverId}.game.lasonpeter.net mc-router mc-router.port **25565** mc-router mc-router.auto-scale-up true (Enable
scale-from-zero) Traefik traefik.enable true Traefik traefik.http.routers.{id}.rule Host(
*{serverId}-console.lasonpeter.net*) Traefik traefik.http.routers.{id}.tls.certresolver myresolver Traefik
traefik.http.services.{id}.loadbalancer.server.port **5000** (SignalR Hub Port) SignalR
traefik.http.services.{id}.loadbalancer.sticky.cookie true (Session persistence)

## Operational Scaling Workflow

Lazy Provisioning: The Manager creates the container but keeps it stopped. Detection: mc-router monitors the Docker
socket. It sees the stopped container and its labels, adding the hostname to its routing table. Scale-Up: When a player
hits the hostname, mc-router issues a StartContainer command to Docker. The wrapper initializes. Routing: Traefik
detects the *Start* event and automatically opens the SignalR/Console route. Scale-Down: After 5 minutes of 0 players,
mc-router stops the container to reclaim **RAM**. ## Wrapper Best Practices Heartbeats: Since Traefik has idle timeouts,
configure your SignalR hub to send frequent heartbeats to keep the WebSocket alive. Resource Caps: Always set NanoCPUs
and Memory limits during container creation to prevent *noisy neighbor* issues. Networking: Use extra_hosts: [
*host.docker.internal:host-gateway*] if the wrapper needs to communicate with the host's localhost.