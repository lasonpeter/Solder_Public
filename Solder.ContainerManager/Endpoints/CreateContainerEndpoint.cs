using FastEndpoints;
using Solder.ContainerManager.Core;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Statics;

namespace Solder.ContainerManager.Endpoints;

public class CreateContainerEndpoint : Endpoint<CreateContainerRequest, CreateServerInstanceResponse>
{
    private readonly IContainerService _containerService;

    public CreateContainerEndpoint(IContainerService containerService)
    {
        _containerService = containerService;
    }

    public override void Configure()
    {
        Post(SolderUris.Container.Create);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateContainerRequest req, CancellationToken ct)
    {
        await _containerService.CreateContainerAsync(req.ServerId, req.ServerName);

        await Send.OkAsync(new CreateServerInstanceResponse(req.ServerId, req.ServerName), ct);
    }
}