using System.Text.Json;
using Solder.Core.DTOs.Instance;
using Solder.Core.DTOs.InstanceRepository;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.LauncherSettings;

namespace Solder.Infrastructure.Persistence.InstanceRepository;

public class InstanceRepository : IInstanceRepository
{
    private readonly ISettingsService _settingsService;
    private InstanceRepositoryData _data = new();

    public InstanceRepository(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _ = LoadInstanceRepositoryAsync();
    }

    public async Task LoadInstanceRepositoryAsync()
    {
        try
        {
            var appPath = _settingsService.GetApplicationPath();
            if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);

            var filePath = Path.Combine(appPath, "InstanceRepository.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                _data = JsonSerializer.Deserialize<InstanceRepositoryData>(json);
            }
            else
            {
                _data = new InstanceRepositoryData();
                await SaveInstanceRepository();
            }

            Console.WriteLine($"Loaded {_data.Instances.Count} instances");
        }
        catch (Exception ex)
        {
            _data = new InstanceRepositoryData();
            await SaveInstanceRepository();
        }
    }

    public async Task SaveInstanceRepository()
    {
        Console.WriteLine("Saving");
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solder");
        var filePath = Path.Combine(folderPath, "InstanceRepository.json");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var options = new JsonSerializerOptions { WriteIndented = true };

        var jsonString = JsonSerializer.Serialize(_data, options);
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine($"Saved {filePath}, \n {jsonString}");
    }

    public Task CreateInstanceRecord(Guid instanceId, string name, string description, string path, string version)
    {
        _data.Instances.Add(new InstanceData
        {
            InstanceId = instanceId,
            Name = name,
            Description = description,
            Path = path,
            Version = version
        });
        Console.WriteLine($"Saving {name} - {description} - {path} - {version}");
        return SaveInstanceRepository();
    }

    public InstanceRepositoryData GetInstances()
    {
        return _data;
    }

    public InstanceData GetInstanceData(Guid instanceId)
    {
        try
        {
            Console.WriteLine($"Getting {instanceId}");
            return _data.Instances.First(data => data.InstanceId == instanceId);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;
        }
    }
}