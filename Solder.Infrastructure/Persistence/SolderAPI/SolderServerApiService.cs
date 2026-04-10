using System.Net.Http.Json;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Statics;

namespace Solder.Infrastructure.Persistence.SolderAPI;

public class SolderServerApiService : ISolderServerApiService
{
    private readonly ISolderAuthService _authService;
    private readonly HttpClient _httpClient;

    public SolderServerApiService(HttpClient httpClient, ISolderAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<IEnumerable<GetServerInstanceResponse>> GetServerInstancesAsync()
    {
        try
        {
            var request =
                await _authService.CreateAuthorizedRequestAsync(HttpMethod.Get, SolderUris.ServerInstance.Get);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<GetServerInstanceResponse>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<GetServerInstanceResponse>>() ??
                   new List<GetServerInstanceResponse>();
        }
        catch
        {
            return new List<GetServerInstanceResponse>();
        }
    }

    public async Task<CreateServerInstanceResponse> CreateServerInstance(CreateServerInstanceRequest requestBody)
    {
        var request =
            await _authService.CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.ServerInstance.Create);
        request.Content = JsonContent.Create(requestBody);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Failed to create server instance.");
        return await response.Content.ReadFromJsonAsync<CreateServerInstanceResponse>() ??
               throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<DeleteServerInstanceResponse> DeleteServerInstance(DeleteServerInstanceRequest requestBody)
    {
        var request =
            await _authService.CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.ServerInstance.Delete);
        request.Content = JsonContent.Create(requestBody);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Failed to delete server instance.");
        return await response.Content.ReadFromJsonAsync<DeleteServerInstanceResponse>() ??
               throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<UpdateServerInstanceResponse> UpdateServerInstance(UpdateServerInstanceRequest requestBody)
    {
        var request =
            await _authService.CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.ServerInstance.Update);
        request.Content = JsonContent.Create(requestBody);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Failed to update server instance.");
        return await response.Content.ReadFromJsonAsync<UpdateServerInstanceResponse>() ??
               throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<GetServerInstanceStateResponse> GetServerInstanceState(GetServerInstanceStateRequest requestBody)
    {
        var request =
            await _authService.CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.ServerInstance.GetState);
        request.Content = JsonContent.Create(requestBody);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Failed to get server instance state.");
        return await response.Content.ReadFromJsonAsync<GetServerInstanceStateResponse>() ??
               throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<ChangeServerInstanceStateResponse> ChangeServerInstanceState(
        ChangeServerInstanceStateRequest requestBody)
    {
        var request =
            await _authService.CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.ServerInstance.SetState);
        request.Content = JsonContent.Create(requestBody);
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to change server instance state.");
        return await response.Content.ReadFromJsonAsync<ChangeServerInstanceStateResponse>() ??
               throw new InvalidOperationException("Failed to deserialize response.");
    }
}