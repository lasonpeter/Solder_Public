using Solder.ServerInstanceManager.Core;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Enums.Solder.ServerInstance;

namespace Solder.ServerInstanceManager.Infrastructure;

public class ServerInstanceService : IServerInstanceService
{
    private readonly State _serverState = new();
    private Guid _projectId;
    private ServerType _serverType;
    private string _version;

    public async Task<GetServerInstanceStateResponse> GetInstanceState()
    {
        return new GetServerInstanceStateResponse(_projectId, _serverState);
    }

    public async Task StartInstance()
    {
    }

    public async Task StopInstance()
    {
        throw new NotImplementedException();
    }

    public async Task SetInstance()
    {
        throw new NotImplementedException();
    }

    public async Task ClearInstance()
    {
        throw new NotImplementedException();
    }
}