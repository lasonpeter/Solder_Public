using Solder.Shared.DTOs.Solder.ServerInstance;

namespace Solder.ServerInstanceManager.Core;

/// <summary>
///     Provides management operations for a single server instance.
/// </summary>
public interface IServerInstanceService
{
    /// <summary>
    ///     Starts the server instance.
    /// </summary>
    /// <returns>A task representing the asynchronous start operation.</returns>
    public Task StartInstance();

    /// <summary>
    ///     Stops the server instance gracefully.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public Task StopInstance();

    /// <summary>
    ///     Configures the server instance with the necessary settings.
    /// </summary>
    /// <returns>A task representing the asynchronous configuration operation.</returns>
    public Task SetInstance();

    /// <summary>
    ///     Clears the current server instance configuration and state.
    /// </summary>
    /// <returns>A task representing the asynchronous clear operation.</returns>
    public Task ClearInstance();

    /// <summary>
    ///     Retrieves the current state and details of the server instance.
    /// </summary>
    /// <returns>A task returning the current state of the server instance.</returns>
    public Task<GetServerInstanceStateResponse> GetInstanceState();
}