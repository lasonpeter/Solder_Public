using Solder.Shared.DTOs.Solder.ServerInstance;

namespace Solder.ServerInstanceManager.Core;

/// <summary>
///     Delegate for handling standard output messages from the server console.
/// </summary>
/// <param name="message">The standard output message data.</param>
public delegate Task StdOutDelegate(ServerInstanceConsoleStdResponse message);

/// <summary>
///     Delegate for handling error output messages from the server console.
/// </summary>
/// <param name="message">The error output message data.</param>
public delegate Task ErrOutDelegate(ServerInstanceConsoleErrResponse message);

/// <summary>
///     Delegate for handling command requests sent to the server console.
/// </summary>
/// <param name="command">The console command request data.</param>
public delegate Task ConsoleCmdDelegate(ServerInstanceConsoleCommandRequest command);

/// <summary>
///     Provides access to the server console input and output streams.
/// </summary>
public interface IConsoleService
{
    /// <summary>
    ///     Gets the delegate for standard output messages.
    /// </summary>
    StdOutDelegate StdOut { get; }

    /// <summary>
    ///     Gets the delegate for error output messages.
    /// </summary>
    ErrOutDelegate ErrOut { get; }

    /// <summary>
    ///     Gets the delegate for processing console command requests.
    /// </summary>
    ConsoleCmdDelegate ConsoleCmdR { get; }
}