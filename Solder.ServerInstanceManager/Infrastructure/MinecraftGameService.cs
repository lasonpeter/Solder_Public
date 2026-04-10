using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;
using Solder.ServerInstanceManager.Core;
using Solder.ServerInstanceManager.Core.APIWrapper;
using Solder.ServerInstanceManager.Core.Persistance;
using Solder.ServerInstanceManager.Serialization;
using Solder.Shared.DTOs.Solder.ServerInstance;
using Solder.Shared.DTOs.Solder.Settings;
using Solder.Shared.Enums.Solder.ServerInstance;
using Console = Colorful.Console;

namespace Solder.ServerInstanceManager.Infrastructure;

public class MinecraftGameService : BackgroundService, IGameService
{
    private const string MvpMinecraftVersion = "1.12.2";
    private const string MvpJavaVersion = "8";
    private readonly IServerInstanceApiService _apiService;

    private readonly IConsoleService _consoleService;
    private readonly IMinecraftJarService _jarService;
    private readonly ILogger<MinecraftGameService> _logger;
    private readonly IServerInstanceInfo _serverInstanceInfo;
    private State _gameState = State.Stopped;
    private StreamWriter _input;

    private Process _process;

    public MinecraftGameService(
        IConsoleService consoleService,
        IServerInstanceApiService apiService,
        IServerInstanceInfo serverInstanceInfo,
        IMinecraftJarService jarService,
        ILogger<MinecraftGameService> logger)
    {
        _consoleService = consoleService;
        _apiService = apiService;
        _serverInstanceInfo = serverInstanceInfo;
        _jarService = jarService;
        _logger = logger;
    }

    public async Task<GetServerInstanceStateResponse> StartGame()
    {
        _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now, "Starting Minecraft..."));
        Console.WriteLine("Starting Minecraft...");
        //Catch 
        if (_gameState != State.Standby && _gameState != State.Stopped && _gameState != State.Created)
            return new GetServerInstanceStateResponse(_serverInstanceInfo.GetServerGuidAsync(), _gameState);

        var response =
            await _apiService.GetServerSettings(new GetSettingsRequest(_serverInstanceInfo.GetServerGuidAsync()));

        //Config load
        StartupConfig startupConfig;
        if (File.Exists("instance_config.json"))
            using (var file = File.OpenRead("instance_config.json"))
            {
                startupConfig = await JsonSerializer.DeserializeAsync(
                    file,
                    AppJsonSerializerContext.Default.StartupConfig
                ) ?? new StartupConfig();
            }
        else
            startupConfig = new StartupConfig();

        if (response != null)
        {
            if (response.Values.TryGetValue("xmx", out var xmx))
                startupConfig.Xmx = xmx;
            if (response.Values.TryGetValue("xms", out var xms))
                startupConfig.Xms = xms;
        }

        // MVP hardcoded runtime targets.
        startupConfig.MinecraftVersion = MvpMinecraftVersion;
        startupConfig.JavaVersion = MvpJavaVersion;

        _logger.LogInformation("Resolving JAR for {Type} {Version}...", startupConfig.ServerType,
            startupConfig.MinecraftVersion);
        _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now,
            $"Resolving JAR for {startupConfig.ServerType} {startupConfig.MinecraftVersion}..."));

        startupConfig.instance_path = await _jarService.EnsureJarAvailableAsync(
            startupConfig.ServerType,
            startupConfig.MinecraftVersion
        );

        _ = _consoleService.StdOut?.Invoke(
            new ServerInstanceConsoleStdResponse(DateTime.Now, $"Running: {startupConfig.instance_path}"));
        Console.WriteLine("Running: " + startupConfig.instance_path, Color.Chartreuse);
        _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now,
            $"Minecraft: {startupConfig.MinecraftVersion} ({startupConfig.ServerType})"));
        Console.WriteLine("Minecraft: " + startupConfig.MinecraftVersion + " (" + startupConfig.ServerType + ")",
            Color.Chartreuse);
        _ = _consoleService.StdOut?.Invoke(
            new ServerInstanceConsoleStdResponse(DateTime.Now, $"Java: {startupConfig.JavaVersion}"));
        Console.WriteLine("Java: " + startupConfig.JavaVersion, Color.Chartreuse);
        _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now,
            $"Running with: {startupConfig.Xmx}MB of allocated RAM"));
        Console.WriteLine("Running with: " + startupConfig.Xmx + "MB of allocated RAM", Color.Chartreuse);
        _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now,
            $"Running with: {startupConfig.Xms}MB of startup RAM"));
        Console.WriteLine("Running with: " + startupConfig.Xms + "MB of startup RAM", Color.Chartreuse);

        if (string.IsNullOrWhiteSpace(startupConfig.working_directory))
            startupConfig.working_directory =
                Path.GetDirectoryName(startupConfig.instance_path) ?? Directory.GetCurrentDirectory();

        EnsureEulaAccepted(startupConfig.working_directory);

        _process = new Process();
        var processStartInfo = new ProcessStartInfo("java",
            "-Xms" + startupConfig.Xms + "M -Xmx" + startupConfig.Xmx + "M -jar " + startupConfig.instance_path +
            " nogui");
        processStartInfo.WorkingDirectory = startupConfig.working_directory;
        _process.StartInfo = processStartInfo;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;
        _process.StartInfo.RedirectStandardInput = true; // Is a MUST!
        _process.EnableRaisingEvents = true;
        _process.OutputDataReceived += (sender, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
                return;

            _ = _consoleService.StdOut?.Invoke(new ServerInstanceConsoleStdResponse(DateTime.Now, args.Data));
            Console.WriteLine(args.Data, Color.DarkGreen);
        };
        _process.ErrorDataReceived += (sender, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
                return;

            _ = _consoleService.ErrOut?.Invoke(new ServerInstanceConsoleErrResponse(DateTime.Now, args.Data));
            Console.WriteLine(args.Data, Color.Red);
        };
        _process.OutputDataReceived += OnOutputDataReceived;

        void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            var pattern = @"\[.*?\d{2}:\d{2}:\d{2}\]?\s?\[Server thread/INFO\].*?Done\s\([\d\.]+s\)!";

            if (args.Data != null && Regex.IsMatch(args.Data, pattern, RegexOptions.IgnoreCase))
            {
                _gameState = State.Running;
                _process.OutputDataReceived -= OnOutputDataReceived;
            }
        }

        _process.Exited += (sender, args) =>
        {
            Console.WriteLine("Server has exited", Color.Red);
            _gameState = State.Stopped;
        };
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        // Non-blocking delay to let the stream initialize
        await Task.Delay(2000);
        _input = _process.StandardInput;
        _input.AutoFlush = true;

        _gameState = State.Starting;

        return new GetServerInstanceStateResponse(_serverInstanceInfo.GetServerGuidAsync(), _gameState);
    }

    public async Task SendCommand(string command)
    {
        if ((_input != null && _gameState == State.Running) || _gameState == State.Starting)
            await _input.WriteLineAsync(command);
    }

    public async Task StopGame()
    {
        throw new NotImplementedException();
    }

    public async Task SetGameState(State state)
    {
        throw new NotImplementedException();
    }

    public async Task<ChangeServerInstanceStateResponse> GetGameState()
    {
        return new ChangeServerInstanceStateResponse(_serverInstanceInfo.GetServerGuidAsync(), _gameState);
    }

    public async Task SetGameConfig()
    {
        throw new NotImplementedException();
    }

    private void EnsureEulaAccepted(string workingDirectory)
    {
        Directory.CreateDirectory(workingDirectory);
        var eulaPath = Path.Combine(workingDirectory, "eula.txt");
        File.WriteAllText(eulaPath, "eula=true" + Environment.NewLine);
        _logger.LogInformation("Ensured EULA acceptance at {EulaPath}", eulaPath);
    }

    private void GracefulShutdown(object? sender, EventArgs e)
    {
        _input.WriteLine("stop");
        _process.WaitForExitAsync().Wait();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("MinecraftGameService starting. Attempting auto-start...");
            var startResponse = await StartGame();
            if (startResponse.ServerInstanceState != State.Starting)
                _logger.LogWarning("Auto-start request was rejected. Current state: {State}",
                    startResponse.ServerInstanceState);

            await Task.Delay(-1, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("END ?????");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while auto-starting MinecraftGameService.");
        }
    }
}
/*
public async Task StopAsync(CancellationToken cancellationToken)
{
    /*await _input.WriteLineAsync("stop");
    await _input.FlushAsync();

    // 2. Wait for the process to actually exit
    // We use the cancellationToken to ensure we don't hang the web server forever
    await _process.WaitForExitAsync(cancellationToken);

    await base.StopAsync(cancellationToken);    }#1#
    return Result;
}*/