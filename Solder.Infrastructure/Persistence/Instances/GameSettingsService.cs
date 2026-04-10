using System.Text.Json;
using Solder.Core.DTOs.Instance.Game;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.Instances;
using Solder.Core.Interfaces.LauncherSettings;

namespace Solder.Infrastructure.Persistence.Instances;

public class GameSettingsService : IGameSettingsService
{
    private readonly IInstanceRepository _instanceRepository;
    private readonly ISettingsService _settingsService;

    public GameSettingsService(IInstanceRepository instanceRepository, ISettingsService settingsService)
    {
        _instanceRepository = instanceRepository;
        _settingsService = settingsService;
    }

    public GameSettings LoadGameSettings(Guid instanceUuid)
    {
        string instancePath;
        try
        {
            instancePath = _instanceRepository.GetInstanceData(instanceUuid).Path;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw new KeyNotFoundException(instanceUuid.ToString());
        }

        try
        {
            var filePath = Path.Combine(instancePath, "settings.json");
            if (!File.Exists(filePath)) return new GameSettings();

            return JsonSerializer.Deserialize<GameSettings>(
                File.ReadAllText(filePath)) ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task SaveGameSettings(Guid instanceUuid, GameSettings settings)
    {
        string instancePath;
        try
        {
            instancePath = _instanceRepository.GetInstanceData(instanceUuid).Path;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new KeyNotFoundException(instanceUuid.ToString());
        }

        try
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            if (!Directory.Exists(instancePath)) Directory.CreateDirectory(instancePath);

            using var fileStream = new FileStream(Path.Combine(instancePath, "settings.json"), FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, settings, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CreateGameSettings(Guid instanceUuid, GameSettings settings)
    {
        var instancePath = Path.Combine(_settingsService.GetInstancePath(), instanceUuid.ToString());
        try
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            /*
            if (Directory.Exists(instancePath))
            {
                throw new InvalidOperationException();
            }*/
            Directory.CreateDirectory(instancePath);
            await using var fileStream = new FileStream(Path.Combine(instancePath, "settings.json"), FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, settings, jsonSerializerOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}