namespace Solder.ContainerManager.Core;

/// <summary>
///     Provides operations for managing Docker containers for server instances.
/// </summary>
public interface IContainerService
{
    /// <summary>
    ///     Asynchronously creates a new container for a server instance.
    /// </summary>
    /// <param name="serverId">The unique ID of the server instance.</param>
    /// <param name="serverName">The name to assign to the server container.</param>
    /// <returns>A task representing the asynchronous creation operation.</returns>
    Task CreateContainerAsync(Guid serverId, string serverName);

    /// <summary>
    ///     Asynchronously starts an existing container for a server instance.
    /// </summary>
    /// <param name="serverId">The unique ID of the server instance.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    Task StartContainerAsync(Guid serverId);

    /// <summary>
    ///     Asynchronously stops a running container for a server instance.
    /// </summary>
    /// <param name="serverId">The unique ID of the server instance.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    Task StopContainerAsync(Guid serverId);

    /// <summary>
    ///     Asynchronously deletes a container for a server instance.
    /// </summary>
    /// <param name="serverId">The unique ID of the server instance.</param>
    /// <returns>A task representing the asynchronous deletion operation.</returns>
    Task DeleteContainerAsync(Guid serverId);
}