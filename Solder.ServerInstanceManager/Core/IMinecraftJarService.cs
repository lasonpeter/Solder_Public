namespace Solder.ServerInstanceManager.Core;

public interface IMinecraftJarService
{
    /// <summary>
    ///     Ensures the specified Minecraft server JAR is available locally.
    ///     Downloads if missing, validates integrity.
    /// </summary>
    /// <param name="type">Server type (Vanilla, Paper, etc.)</param>
    /// <param name="version">Minecraft version (e.g., "1.20.1")</param>
    /// <returns>Absolute path to the JAR file</returns>
    Task<string> EnsureJarAvailableAsync(string type, string version);

    /// <summary>
    ///     Gets available Minecraft versions for a specific server type.
    /// </summary>
    /// <param name="type">Server type</param>
    /// <returns>List of available versions</returns>
    Task<IEnumerable<MinecraftVersion>> GetAvailableVersionsAsync(string type);

    /// <summary>
    ///     Removes unused JARs based on last access time and usage count.
    /// </summary>
    /// <param name="olderThanDays">Remove JARs not used in X days</param>
    /// <returns>Number of JARs removed</returns>
    Task<int> CleanupUnusedJarsAsync(int olderThanDays = 30);

    /// <summary>
    ///     Gets the required Java version for a specific Minecraft version.
    /// </summary>
    /// <param name="minecraftVersion">Minecraft version (e.g., "1.20.1")</param>
    /// <returns>Java major version (8, 17, 21)</returns>
    int GetRequiredJavaVersion(string minecraftVersion);
}

public record MinecraftVersion(string Version, string Type, DateTime ReleasedAt, string JavaVersion);