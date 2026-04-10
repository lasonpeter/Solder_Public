using System.Text.Json;
using Solder.ServerInstanceManager.Core;

namespace Solder.ServerInstanceManager.Infrastructure;

public class MinecraftJarService : IMinecraftJarService
{
    private const string MojangVersionManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
    private const string PaperApiUrl = "https://api.papermc.io/v2/projects/paper";
    private readonly HttpClient _httpClient;
    private readonly string _jarStoragePath;
    private readonly ILogger<MinecraftJarService> _logger;

    public MinecraftJarService(HttpClient httpClient, ILogger<MinecraftJarService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jarStoragePath = configuration["MinecraftJarService:StoragePath"] ?? "/var/solder/jars";

        // Ensure storage directory exists
        Directory.CreateDirectory(_jarStoragePath);
    }

    public async Task<string> EnsureJarAvailableAsync(string type, string version)
    {
        var jarPath = GetJarPath(type, version);

        if (File.Exists(jarPath))
        {
            _logger.LogInformation("JAR already exists: {JarPath}", jarPath);
            return jarPath;
        }

        _logger.LogInformation("Downloading {Type} {Version}...", type, version);

        var downloadUrl = type.ToLower() switch
        {
            "vanilla" => await GetVanillaDownloadUrlAsync(version),
            "paper" => await GetPaperDownloadUrlAsync(version),
            "spigot" => await GetSpigotDownloadUrlAsync(version),
            _ => throw new NotSupportedException($"Server type '{type}' is not supported")
        };

        await DownloadJarAsync(downloadUrl, jarPath);

        _logger.LogInformation("Successfully downloaded {Type} {Version} to {JarPath}", type, version, jarPath);
        return jarPath;
    }

    public async Task<IEnumerable<MinecraftVersion>> GetAvailableVersionsAsync(string type)
    {
        return type.ToLower() switch
        {
            "vanilla" => await GetVanillaVersionsAsync(),
            "paper" => await GetPaperVersionsAsync(),
            _ => throw new NotSupportedException($"Server type '{type}' is not supported")
        };
    }

    public async Task<int> CleanupUnusedJarsAsync(int olderThanDays = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var removed = 0;

        if (!Directory.Exists(_jarStoragePath))
            return 0;

        foreach (var jarFile in Directory.GetFiles(_jarStoragePath, "*.jar", SearchOption.AllDirectories))
        {
            var lastAccess = File.GetLastAccessTime(jarFile);
            if (lastAccess < cutoffDate)
                try
                {
                    File.Delete(jarFile);
                    removed++;
                    _logger.LogInformation("Removed unused JAR: {JarFile}", jarFile);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to remove JAR: {JarFile}", jarFile);
                }
        }

        return removed;
    }

    public int GetRequiredJavaVersion(string minecraftVersion)
    {
        // Parse version (e.g., "1.20.1" -> major: 1, minor: 20)
        var parts = minecraftVersion.Split('.');
        if (parts.Length < 2 || !int.TryParse(parts[1], out var minorVersion))
            return 21; // Default to latest

        // Minecraft version -> Java version mapping
        return minorVersion switch
        {
            <= 16 => 8, // 1.16.5 and below
            17 => 17, // 1.17.x
            >= 20 => 21, // 1.20.5+ (Adjust logic as needed for specific sub-versions)
            _ => 17 // Covers 18, 19, and anything else not caught above
        };
    }

    private string GetJarPath(string type, string version)
    {
        var directory = Path.Combine(_jarStoragePath, version, type.ToLower());
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"server-{version}.jar");
    }

    private async Task<string> GetVanillaDownloadUrlAsync(string version)
    {
        var manifestResponse = await _httpClient.GetStringAsync(MojangVersionManifestUrl);
        var manifest = JsonDocument.Parse(manifestResponse);

        var versionEntry = manifest.RootElement
            .GetProperty("versions")
            .EnumerateArray()
            .FirstOrDefault(v => v.GetProperty("id").GetString() == version);

        if (versionEntry.ValueKind == JsonValueKind.Undefined)
            throw new InvalidOperationException($"Minecraft version {version} not found");

        var versionUrl = versionEntry.GetProperty("url").GetString();
        var versionData = await _httpClient.GetStringAsync(versionUrl);
        var versionDoc = JsonDocument.Parse(versionData);

        return versionDoc.RootElement
            .GetProperty("downloads")
            .GetProperty("server")
            .GetProperty("url")
            .GetString()!;
    }

    private async Task<string> GetPaperDownloadUrlAsync(string version)
    {
        // Get latest build for version
        var buildsUrl = $"{PaperApiUrl}/versions/{version}/builds";
        var buildsResponse = await _httpClient.GetStringAsync(buildsUrl);
        var builds = JsonDocument.Parse(buildsResponse);

        var latestBuild = builds.RootElement
            .GetProperty("builds")
            .EnumerateArray()
            .Last()
            .GetProperty("build")
            .GetInt32();

        var fileName = builds.RootElement
            .GetProperty("builds")
            .EnumerateArray()
            .Last()
            .GetProperty("downloads")
            .GetProperty("application")
            .GetProperty("name")
            .GetString();

        return $"{PaperApiUrl}/versions/{version}/builds/{latestBuild}/downloads/{fileName}";
    }

    private Task<string> GetSpigotDownloadUrlAsync(string version)
    {
        // Spigot requires BuildTools - for now, return a placeholder or error
        throw new NotImplementedException("Spigot downloads require BuildTools. Use Paper or Vanilla.");
    }

    private async Task<IEnumerable<MinecraftVersion>> GetVanillaVersionsAsync()
    {
        var manifestResponse = await _httpClient.GetStringAsync(MojangVersionManifestUrl);
        var manifest = JsonDocument.Parse(manifestResponse);

        return manifest.RootElement
            .GetProperty("versions")
            .EnumerateArray()
            .Where(v => v.GetProperty("type").GetString() == "release")
            .Take(20) // Last 20 releases
            .Select(v =>
            {
                var version = v.GetProperty("id").GetString()!;
                var releaseTime = v.GetProperty("releaseTime").GetDateTime();
                var javaVersion = GetRequiredJavaVersion(version).ToString();
                return new MinecraftVersion(version, "Vanilla", releaseTime, javaVersion);
            })
            .ToList();
    }

    private async Task<IEnumerable<MinecraftVersion>> GetPaperVersionsAsync()
    {
        var versionsUrl = $"{PaperApiUrl}/versions";
        var versionsResponse = await _httpClient.GetStringAsync(versionsUrl);
        var versions = JsonDocument.Parse(versionsResponse);

        return versions.RootElement
            .GetProperty("versions")
            .EnumerateArray()
            .Take(20)
            .Select(v =>
            {
                var version = v.GetString()!;
                var javaVersion = GetRequiredJavaVersion(version).ToString();
                return new MinecraftVersion(version, "Paper", DateTime.UtcNow, javaVersion);
            })
            .ToList();
    }

    private async Task DownloadJarAsync(string url, string destinationPath)
    {
        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fileStream);
    }
}