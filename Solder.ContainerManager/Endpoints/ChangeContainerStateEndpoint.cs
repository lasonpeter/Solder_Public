using FastEndpoints;
using Solder.ContainerManager.Core;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Enums.Solder.ServerInstance;
using Solder.Shared.Statics;

namespace Solder.ContainerManager.Endpoints;

public class
    ChangeContainerStateEndpoint : Endpoint<ChangeServerInstanceStateRequest, ChangeServerInstanceStateResponse>
{
    private readonly IContainerService _containerService;

    public ChangeContainerStateEndpoint(IContainerService containerService)
    {
        _containerService = containerService;
    }

    public override void Configure()
    {
        Post(SolderUris.Container.State);
        AllowAnonymous();
    }

    public override async Task HandleAsync(ChangeServerInstanceStateRequest req, CancellationToken ct)
    {
        switch (req.ServerInstanceState)
        {
            case State.Running:
                await _containerService.StartContainerAsync(req.ServerInstanceId);
                break;
            case State.Stopped:
                await _containerService.StopContainerAsync(req.ServerInstanceId);
                break;
            default:
                await Send.NotFoundAsync(ct);
                return;
        }

        await Send.OkAsync(new ChangeServerInstanceStateResponse(req.ServerInstanceId, req.ServerInstanceState), ct);
    }
}