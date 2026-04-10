using Solder.Core.DTOs.Api.Solder;

namespace Solder.Core.Interfaces.Api.Solder;

public interface ISolderApiService
{
    // Projects
    Task<IEnumerable<ProjectResponse>> GetProjectsAsync();
    Task<ProjectResponse?> CreateProjectAsync(CreateProjectRequest request);
    Task<ProjectResponse?> UpdateProjectAsync(UpdateProjectRequest request);
    Task<bool> DeleteProjectAsync(string projectId);
}