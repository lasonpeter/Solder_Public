using Solder.Core.DTOs.Api.Solder;

namespace Solder.Core.Interfaces.Api.Solder;

/// <summary>
///     Provides project-related operations via the Solder API.
/// </summary>
public interface ISolderProjectApiService
{
    /// <summary>
    ///     Retrieves all projects belonging to the authenticated user.
    /// </summary>
    /// <returns>A collection of project responses.</returns>
    public Task<IEnumerable<ProjectResponse>> GetProjectsAsync();

    /// <summary>
    ///     Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request data.</param>
    /// <returns>The created project response if successful; otherwise, null.</returns>
    public Task<ProjectResponse?> CreateProjectAsync(CreateProjectRequest request);

    /// <summary>
    ///     Updates an existing project.
    /// </summary>
    /// <param name="request">The project update request data.</param>
    /// <returns>The updated project response if successful; otherwise, null.</returns>
    public Task<ProjectResponse?> UpdateProjectAsync(UpdateProjectRequest request);

    /// <summary>
    ///     Deletes an existing project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project to delete.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    public Task<bool> DeleteProjectAsync(string projectId);
}