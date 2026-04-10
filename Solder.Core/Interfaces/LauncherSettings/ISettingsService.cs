namespace Solder.Core.Interfaces.LauncherSettings;

/// <summary>
///     Provides access to various application and game-related paths and settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    ///     Gets the path where instances are stored.
    /// </summary>
    /// <returns>The path to the instances directory.</returns>
    string GetInstancePath();

    /// <summary>
    ///     Gets the path to the game executable or base directory.
    /// </summary>
    /// <returns>The game path.</returns>
    string GetGamePath();

    /// <summary>
    ///     Sets the path to the game executable or base directory.
    /// </summary>
    /// <returns>The newly set game path.</returns>
    string SetGamePath();

    /// <summary>
    ///     Gets the path to the temporary files directory.
    /// </summary>
    /// <returns>The temporary file path.</returns>
    string GetTempFilePath();

    /// <summary>
    ///     Sets the path to the temporary files directory.
    /// </summary>
    /// <returns>The newly set temporary file path.</returns>
    string SetTempFilePath();

    /// <summary>
    ///     Gets the base path of the application.
    /// </summary>
    /// <returns>The application base path.</returns>
    string GetApplicationPath();
}