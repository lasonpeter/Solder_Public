using Microsoft.AspNetCore.SignalR;
using Solder.ServerInstanceManager.Core;
using Solder.ServerInstanceManager.Endpoints.SignalR;

namespace Solder.ServerInstanceManager.Infrastructure;

public class ConsoleService : IConsoleService
{
    private readonly IHubContext<ConsoleHub> _consoleHub;

    public ConsoleService(IHubContext<ConsoleHub> consoleHub)
    {
        _consoleHub = consoleHub;

        StdOut = async message => { await _consoleHub.Clients.All.SendAsync("ReceiveStdOutput", message); };

        ErrOut = async message => { await _consoleHub.Clients.All.SendAsync("ReceiveErrOutput", message); };
    }

    public StdOutDelegate StdOut { get; }
    public ErrOutDelegate ErrOut { get; }
    public ConsoleCmdDelegate ConsoleCmdR { get; } = null!;
}