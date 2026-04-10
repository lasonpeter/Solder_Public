using Solder.ServerInstanceManager.Core.APIWrapper;
using Solder.ServerInstanceManager.Serialization;
using Solder.Shared.DTOs.Solder.Settings;
using static Solder.Shared.Statics.SolderUris;

namespace Solder.ServerInstanceManager.Infrastructure.APIWrapper;

public class ServerInstanceApiService : IServerInstanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServerInstanceApiService> _logger;

    public ServerInstanceApiService(HttpClient httpClient, IConfiguration configuration,
        ILogger<ServerInstanceApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        if (_httpClient.BaseAddress != null)
            return;

        var configuredBaseUrl = configuration["SolderApi:BaseUrl"];
        var baseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl)
            ? FallbackBaseUrl
            : configuredBaseUrl;

        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger.LogInformation("ServerInstance API base URL: {BaseUrl}", _httpClient.BaseAddress);
    }

    public async Task<GetSettingsResponse?> GetServerSettings(GetSettingsRequest request)
    {
        var requestUri = $"{ServerInstance.GetSettings}?serverId={request.ServerId}";
        var response = await _httpClient.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            var getSettingsResponse = await response.Content.ReadFromJsonAsync(
                AppJsonSerializerContext.Default.GetSettingsResponse
            );
            if (getSettingsResponse == null)
                return null;

            return getSettingsResponse;
        }

        _logger.LogWarning("Failed to fetch server settings. Status code: {StatusCode}", response.StatusCode);
        return null;
    }
}