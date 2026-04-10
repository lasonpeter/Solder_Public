using Solder.ServerInstanceManager.Core;
using Solder.ServerInstanceManager.Core.APIWrapper;
using Solder.ServerInstanceManager.Core.Persistance;
using Solder.ServerInstanceManager.Endpoints.SignalR;
using Solder.ServerInstanceManager.Exceptions;
using Solder.ServerInstanceManager.Infrastructure;
using Solder.ServerInstanceManager.Infrastructure.APIWrapper;
using Solder.ServerInstanceManager.Infrastructure.Persistance;

namespace Solder.ServerInstanceManager;

public class Program
{
    public static void Main(string[] args)
    {
        var bld = WebApplication.CreateBuilder(args);

        // Validate SERVER_INSTANCE_ID is provided
        var instanceId = bld.Configuration["SERVER_INSTANCE_ID"];
        if (string.IsNullOrEmpty(instanceId) || !Guid.TryParse(instanceId, out _))
            throw new InvalidOperationException(
                "SERVER_INSTANCE_ID environment variable is required and must be a valid GUID. " +
                "Each container must be started with a unique instance ID.");

        Console.WriteLine($"Starting ServerInstanceManager for instance: {instanceId}");

        bld.Services.AddExceptionHandler<GlobalExceptionHandler>();
        bld.Services.AddSingleton<IConsoleService, ConsoleService>();
        bld.Services.AddSingleton<IServerInstanceInfo, ServerInstanceInfoService>();
        bld.Services.AddScoped<IServerInstanceService, ServerInstanceService>();
        bld.Services.AddHttpClient<IServerInstanceApiService, ServerInstanceApiService>();
        bld.Services.AddSignalR();

        bld.Services.AddHttpClient<IMinecraftJarService, MinecraftJarService>();
        bld.Services.AddSingleton<IMinecraftJarService, MinecraftJarService>();

        bld.Services.AddHostedService<MinecraftGameService>();
        bld.Services.AddSingleton<IGameService>(sp => sp.GetRequiredService<MinecraftGameService>());

        bld.Services.AddProblemDetails();

        var app = bld.Build();

        app.UseExceptionHandler();
        app.MapHub<ConsoleHub>("/console");
        app.Run();
    }
}