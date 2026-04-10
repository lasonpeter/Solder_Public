using FastEndpoints;
using Solder.ContainerManager.Core;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Statics;

namespace Solder.ContainerManager.Endpoints;

public class DeleteContainerEndpoint : Endpoint<DeleteServerInstanceRequest, DeleteServerInstanceResponse>
{
    private readonly IContainerService _containerService;

    public DeleteContainerEndpoint(IContainerService containerService)
    {
        _containerService = containerService;
    }

    public override void Configure()
    {
        Post(SolderUris.Container.Delete);
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteServerInstanceRequest req, CancellationToken ct)
    {
        await _containerService.DeleteContainerAsync(req.ServerId);
        await Send.OkAsync(new DeleteServerInstanceResponse(), ct);
    }
}