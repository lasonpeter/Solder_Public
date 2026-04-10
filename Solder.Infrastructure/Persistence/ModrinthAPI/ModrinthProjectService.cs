using Solder.Core.DTOs.Modrinth;
using Solder.Core.Interfaces.ModrinthAPI.Project;

namespace Solder.Infrastructure.Persistence.ModrinthAPI;

public class ModrinthProjectService : BaseModrinthApiService, IModrinthProjectService
{
    public ModrinthProjectService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<ModrinthSearchResponse?> SearchProjectByName(string projectName,
        Action<ModrinthQueryBuilder>? query = null)
    {
        var builder = new ModrinthQueryBuilder();
        builder.Add("query", projectName); // Actually add the search query
        query?.Invoke(builder); // Run the user's "sugar" logic
        return await SendRequestAsync<ModrinthSearchResponse>(ModrinthUris.SearchProjectUrl + builder.Build());
    }

    public Task SearchProjectById(string projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<ModrinthSearchResponse?> GetProjectById(string projectId)
    {
        return await SendRequestAsync<ModrinthSearchResponse>($"{ModrinthUris.GetProject} \\ {projectId}");
    }
}