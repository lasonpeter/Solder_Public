using Docker.DotNet;
using Docker.DotNet.Models;
using Solder.ContainerManager.Core;

namespace Solder.ContainerManager.Infrastructure;

public class DockerContainerService : IContainerService
{
    private readonly IConfiguration _configuration;
    private readonly IDockerClient _dockerClient;

    public DockerContainerService(IDockerClient dockerClient, IConfiguration configuration)
    {
        _dockerClient = dockerClient;
        _configuration = configuration;
    }

    private string ImageName => _configuration["Docker:ImageName"] ?? "solder-sim:java8-aot";
    private string NetworkName => _configuration["Docker:NetworkName"] ?? "mc-network";
    private string Domain => _configuration["Docker:Domain"] ?? "lvh.me";
    private string SolderApiBaseUrl => _configuration["Docker:SolderApiBaseUrl"] ?? "http://host.docker.internal:5015";

    public async Task CreateContainerAsync(Guid serverId, string serverName)
    {
        var serverIdStr = serverId.ToString().ToLower();
        var containerName = $"solder-instance-{serverIdStr}";

        var parameters = new CreateContainerParameters
        {
            Image = ImageName,
            Name = containerName,
            Env = new List<string>
            {
                $"SERVER_INSTANCE_ID={serverIdStr}",
                "ASPNETCORE_URLS=http://+:8080", // Ensure it matches what Traefik expects
                $"SolderApi__BaseUrl={SolderApiBaseUrl}"
            },
            Labels = new Dictionary<string, string>
            {
                { "mc-router.host", $"{serverIdStr}.game.{Domain}" },
                { "mc-router.port", "25565" },
                { "mc-router.auto-scale-up", "true" },
                { "mc-router.auto-scale-down-delay", "5m" },
                { "traefik.enable", "true" },
                { $"traefik.http.routers.{serverIdStr}.rule", $"Host(`{serverIdStr}-console.{Domain}`)" },
                { $"traefik.http.services.{serverIdStr}.loadbalancer.server.port", "8080" },
                { $"traefik.http.services.{serverIdStr}.loadbalancer.sticky.cookie", "true" }
            },

            HostConfig = new HostConfig
            {
                NetworkMode = NetworkName,
                // Resource limits as per overview.md best practices
                NanoCPUs = 2000000000, // 1 CPU
                Memory = GB(2),
                ExtraHosts = new List<string> { "host.docker.internal:host-gateway" }
            },
            NetworkingConfig = new NetworkingConfig
            {
                EndpointsConfig = new Dictionary<string, EndpointSettings>
                {
                    { NetworkName, new EndpointSettings() }
                }
            }
        };

        await _dockerClient.Containers.CreateContainerAsync(parameters);
    }

    public async Task StartContainerAsync(Guid serverId)
    {
        var containerName = $"solder-instance-{serverId.ToString().ToLower()}";
        await _dockerClient.Containers.StartContainerAsync(containerName, new ContainerStartParameters());
    }

    public async Task StopContainerAsync(Guid serverId)
    {
        var containerName = $"solder-instance-{serverId.ToString().ToLower()}";
        await _dockerClient.Containers.StopContainerAsync(containerName, new ContainerStopParameters());
    }

    public async Task DeleteContainerAsync(Guid serverId)
    {
        var containerName = $"solder-instance-{serverId.ToString().ToLower()}";
        await _dockerClient.Containers.RemoveContainerAsync(containerName,
            new ContainerRemoveParameters { Force = true });
    }

    private static long GB(int n)
    {
        return (long)n * 1024 * 1024 * 1024;
    }
}