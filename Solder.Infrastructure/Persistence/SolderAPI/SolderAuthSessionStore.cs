using System.Text.Json;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Core.Interfaces.LauncherSettings;
using Solder.Core.Models;

namespace Solder.Infrastructure.Persistence.SolderAPI;

/// <summary>
///     Desktop implementation of the <see cref="ISolderAuthSessionStore" /> that persists the session to a JSON file.
/// </summary>
public class SolderAuthSessionStore : ISolderAuthSessionStore
{
    private const string SessionFileName = "auth_session.json";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        { WriteIndented = true };

    private readonly ISettingsService _settingsService;

    public SolderAuthSessionStore(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <inheritdoc />
    public async Task<AuthSession?> GetSessionAsync()
    {
        try
        {
            var filePath = GetSessionFilePath();
            if (!File.Exists(filePath)) return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<AuthSession>(json, SerializerOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading auth session: {ex.Message}");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(AuthSession session)
    {
        try
        {
            var filePath = GetSessionFilePath();
            var json = JsonSerializer.Serialize(session, SerializerOptions);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving auth session: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        try
        {
            var filePath = GetSessionFilePath();
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing auth session: {ex.Message}");
        }
    }

    private string GetSessionFilePath()
    {
        var appPath = _settingsService.GetApplicationPath();
        if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
        return Path.Combine(appPath, SessionFileName);
    }
}