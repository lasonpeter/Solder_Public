using Microsoft.AspNetCore.SignalR;
using Solder.ServerInstanceManager.Core;
using Solder.Shared.DTOs.Solder.ServerInstance;

namespace Solder.ServerInstanceManager.Endpoints.SignalR;

public class ConsoleHub : Hub
{
    private readonly IGameService _gameService;

    public ConsoleHub(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task InvokeServerCommand(ServerInstanceConsoleCommandRequest message)
    {
        await _gameService.SendCommand(message.Message);
    }
}