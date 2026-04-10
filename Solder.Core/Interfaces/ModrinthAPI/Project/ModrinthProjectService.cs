using Solder.Core.DTOs.Modrinth;
using Solder.Infrastructure.Persistence.ModrinthAPI;

namespace Solder.Core.Interfaces.ModrinthAPI.Project;

public interface IModrinthProjectService
{
    /// <summary>
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public Task<ModrinthSearchResponse?> SearchProjectByName(string projectName,
        Action<ModrinthQueryBuilder>? query = null);

    public Task SearchProjectById(string projectId);
    public Task<ModrinthSearchResponse?> GetProjectById(string projectId);
}