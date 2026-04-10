using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using Solder.Core.DTOs.Instance;
using Solder.Core.DTOs.Instance.Game;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.Instances;
using Solder.Core.Interfaces.LauncherSettings;

namespace Solder.Infrastructure.Persistence.Instances;

public class InstanceService : IInstanceService
{
    private readonly IGameSettingsService _gameSettingsService;
    private readonly IInstanceRepository _instanceRepository;


    private readonly ISettingsService _settingsService;

    public InstanceService(ISettingsService settingsService, IInstanceRepository instanceRepository,
        IGameSettingsService gameSettingsService)
    {
        _settingsService = settingsService;
        _instanceRepository = instanceRepository;
        _gameSettingsService = gameSettingsService;
    }

    public event IInstanceService.InstanceCreationProgressChanged? OnInstanceCreationProgressChanged;

    public async Task CreateInstance(InstanceCreationData instanceCreationData)
    {
        var guid = Guid.NewGuid();
        //create directories for GameFolder and also instance related things, IE: settings, backups, and other data and metadata
        var directoryInfo =
            new DirectoryInfo(Path.Combine(Path.Combine(_settingsService.GetInstancePath(), guid.ToString()),
                "GameFolder"));
        if (!directoryInfo.Exists) directoryInfo.Create();

        //Create necessary metadata and configs
        var gameSettings = new GameSettings();
        await _gameSettingsService.CreateGameSettings(guid, gameSettings);


        var minecraftPath =
            new MinecraftPath(Path.Combine(Path.Combine(_settingsService.GetInstancePath(), guid.ToString()),
                "GameFolder"));
        var launcher = new MinecraftLauncher(minecraftPath);
        launcher.ByteProgressChanged += (sender, args) =>
        {
            Console.WriteLine(args.ProgressedBytes / 1024 + " / " + args.TotalBytes / 1024);
            OnInstanceCreationProgressChanged?.Invoke(guid, instanceCreationData.Name, args.ProgressedBytes,
                args.TotalBytes);
        };
        /*launcher.FileProgressChanged += (sender, args) =>
        {
            Console.WriteLine(args.ProgressedTasks + " / " + args.TotalTasks);
        };*/
        var versionToInstall = instanceCreationData.Version;
        if (versionToInstall == "latest")
        {
            var versions = await launcher.GetAllVersionsAsync();
            versionToInstall = versions.LatestReleaseName ?? "1.20.1";
        }

        await launcher.InstallAsync(versionToInstall);

        Console.WriteLine($"Created {Path.Combine(_settingsService.GetInstancePath(), guid.ToString())}");

        await _instanceRepository.CreateInstanceRecord(guid, instanceCreationData.Name,
            instanceCreationData.Description, Path.Combine(_settingsService.GetInstancePath(), guid.ToString()),
            instanceCreationData.Version);
    }

    public async Task RunInstanceAsync(Guid instanceId)
    {
        //Load game settings


        Console.WriteLine("Starting instance {0}", instanceId);
        var instanceData = _instanceRepository.GetInstanceData(instanceId);
        var gameSettings = _gameSettingsService.LoadGameSettings(instanceId);

        var launcher = new MinecraftLauncher(Path.Combine(instanceData.Path, "GameFolder"));
        var version = await launcher.GetVersionAsync(instanceData.Version);
        var javaPath = string.IsNullOrEmpty(gameSettings.JavaPath)
            ? launcher.GetJavaPath(version)
            : gameSettings.JavaPath;
        Console.WriteLine(javaPath);
        Console.WriteLine("STARTING INSTANCE");

        var e = await launcher.BuildProcessAsync(instanceData.Version, new MLaunchOption
        {
            MaximumRamMb = (int)gameSettings.Xmx,
            MinimumRamMb = (int)gameSettings.Xms,
            Session = MSession.CreateOfflineSession("Wehe")
        });

        var processWrapper = new ProcessWrapper(e);
        processWrapper.OutputReceived += (s, e) => Console.WriteLine($"[Game] {e}");
        processWrapper.StartWithEvents();
        var exitCode = await processWrapper.WaitForExitTaskAsync();
        Console.WriteLine($"Exited with code {exitCode}");
    }

    public void DeleteInstance(Guid instanceId)
    {
        throw new NotImplementedException();
    }
}