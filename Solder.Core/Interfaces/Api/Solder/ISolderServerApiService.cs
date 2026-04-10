using Solder.Shared.DTOs.Solder.ServerInstance;

namespace Solder.Core.Interfaces.Api.Solder;

public interface ISolderServerApiService
{
    public Task<IEnumerable<GetServerInstanceResponse>> GetServerInstancesAsync();
    public Task<CreateServerInstanceResponse> CreateServerInstance(CreateServerInstanceRequest request);
    public Task<DeleteServerInstanceResponse> DeleteServerInstance(DeleteServerInstanceRequest request);
    public Task<UpdateServerInstanceResponse> UpdateServerInstance(UpdateServerInstanceRequest request);
    public Task<GetServerInstanceStateResponse> GetServerInstanceState(GetServerInstanceStateRequest request);
    public Task<ChangeServerInstanceStateResponse> ChangeServerInstanceState(ChangeServerInstanceStateRequest request);
}