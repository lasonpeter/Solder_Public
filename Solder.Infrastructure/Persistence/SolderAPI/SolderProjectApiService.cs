using System.Net.Http.Json;
using Solder.Core.DTOs.Api.Solder;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Shared.Statics;

namespace Solder.Infrastructure.Persistence.SolderAPI;

public class SolderProjectApiService : ISolderProjectApiService
{
    private readonly HttpClient _httpClient;

    public SolderProjectApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProjectResponse>> GetProjectsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProjectResponse>>(SolderUris.Project.Get) ??
                   new List<ProjectResponse>();
        }
        catch
        {
            return new List<ProjectResponse>();
        }
    }

    public async Task<ProjectResponse?> CreateProjectAsync(CreateProjectRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Create, request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ProjectResponse>();
        return null;
    }

    public async Task<ProjectResponse?> UpdateProjectAsync(UpdateProjectRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Update, request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ProjectResponse>();
        return null;
    }

    public async Task<bool> DeleteProjectAsync(string projectId)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Delete, new { ProjectID = projectId });
        return response.IsSuccessStatusCode;
    }
}