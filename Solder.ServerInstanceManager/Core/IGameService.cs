using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.Enums.Solder.ServerInstance;

namespace Solder.ServerInstanceManager.Core;

/// <summary>
///     Provides operations to control and monitor the game server process.
/// </summary>
public interface IGameService
{
    /// <summary>
    ///     Starts the game server process.
    /// </summary>
    /// <returns>A response containing the initial state after starting.</returns>
    public Task<GetServerInstanceStateResponse> StartGame();

    /// <summary>
    ///     Stops the game server process.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public Task StopGame();

    /// <summary>
    ///     Updates the current state of the game server.
    /// </summary>
    /// <param name="state">The new state to set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetGameState(State state);

    /// <summary>
    ///     Retrieves the current state of the game server.
    /// </summary>
    /// <returns>A response containing the current server state.</returns>
    public Task<ChangeServerInstanceStateResponse> GetGameState();

    /// <summary>
    ///     Configures the game server settings and environment.
    /// </summary>
    /// <returns>A task representing the asynchronous configuration operation.</returns>
    public Task SetGameConfig();

    /// <summary>
    ///     Sends a command to the running game server process.
    /// </summary>
    /// <param name="command">The command string to execute.</param>
    /// <returns>A task representing the asynchronous command execution.</returns>
    public Task SendCommand(string command);
}