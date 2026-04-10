using Solder.Shared.DTOs.Solder.Settings;

namespace Solder.ServerInstanceManager.Core.APIWrapper;

public interface IServerInstanceApiService
{
    public Task<GetSettingsResponse?> GetServerSettings(GetSettingsRequest request);
}