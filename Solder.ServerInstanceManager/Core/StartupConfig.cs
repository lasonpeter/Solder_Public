namespace Solder.ServerInstanceManager.Core;

public class StartupConfig
{
    /// <summary>
    ///     Minecraft version (e.g., "1.20.1")
    /// </summary>
    public string MinecraftVersion { get; set; } = "1.12.2";

    /// <summary>
    ///     Server type (e.g., "vanilla", "paper", "spigot")
    /// </summary>
    public string ServerType { get; set; } = "vanilla";

    /// <summary>
    ///     Required Java version (e.g., "8", "17", "21")
    ///     Auto-detected if not specified
    /// </summary>
    public string? JavaVersion { get; set; } = "8";

    /// <summary>
    ///     Path to server JAR (auto-resolved if not specified)
    /// </summary>
    public string instance_path { get; set; } = string.Empty;

    /// <summary>
    ///     Maximum heap size in MB
    /// </summary>
    public string Xmx { get; set; } = "1024";

    /// <summary>
    ///     Initial heap size in MB
    /// </summary>
    public string Xms { get; set; } = "1024";

    /// <summary>
    ///     Working directory for server instance
    /// </summary>
    public string working_directory { get; set; } = string.Empty;
}