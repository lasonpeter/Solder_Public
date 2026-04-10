using Solder.Core.DTOs.Instance.Game;

namespace Solder.Core.Interfaces.Instances;

public interface IGameSettingsService
{
    /// <summary>
    ///     Loads game settings from file
    /// </summary>
    /// <param name="instanceUuid">Self explanatory I guess</param>
    /// <returns></returns>
    GameSettings LoadGameSettings(Guid instanceUuid);

    /// <summary>
    ///     Saves game settings to a file
    /// </summary>
    /// <param name="instanceUuid">Self explanatory I guess</param>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveGameSettings(Guid instanceUuid, GameSettings settings);

    /// <summary>
    ///     Saves game settings to a file
    /// </summary>
    /// <param name="instanceUuid">Self explanatory I guess</param>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task CreateGameSettings(Guid instanceUuid, GameSettings settings);
}