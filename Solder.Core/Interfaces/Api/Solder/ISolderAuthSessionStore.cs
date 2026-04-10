using Solder.Core.Models;

namespace Solder.Core.Interfaces.Api.Solder;

public interface ISolderAuthSessionStore
{
    public Task<AuthSession?> GetSessionAsync();
    public Task SaveAsync(AuthSession session);
    public Task ClearAsync();
}