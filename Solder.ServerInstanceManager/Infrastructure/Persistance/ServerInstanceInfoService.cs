using Solder.ServerInstanceManager.Core.Persistance;

namespace Solder.ServerInstanceManager.Infrastructure.Persistance;

public class ServerInstanceInfoService : IServerInstanceInfo
{
    private readonly Guid _serverInstanceId;

    public ServerInstanceInfoService(IConfiguration configuration)
    {
        var idStr = configuration["SERVER_INSTANCE_ID"];
        if (string.IsNullOrEmpty(idStr) || !Guid.TryParse(idStr, out _serverInstanceId))
            throw new InvalidOperationException("SERVER_INSTANCE_ID environment variable is missing or invalid.");
    }

    public Guid GetServerGuidAsync()
    {
        return _serverInstanceId;
    }
}