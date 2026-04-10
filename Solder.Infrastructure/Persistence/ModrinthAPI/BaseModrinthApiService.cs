using System.Net.Http.Json;

namespace Solder.Infrastructure.Persistence.ModrinthAPI;

public abstract class BaseModrinthApiService
{
    protected readonly HttpClient _httpClient;

    protected int RateLimit = 100;
    protected int RateLimitRemaining = 100;
    protected int RateLimitResetTiming = 100;

    protected BaseModrinthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        if (_httpClient.BaseAddress == null)
            _httpClient.BaseAddress =
                new Uri(ModrinthUris.BaseUrl.EndsWith("/") ? ModrinthUris.BaseUrl : ModrinthUris.BaseUrl + "/");

        // Modrinth REQUIRES a unique User-Agent
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            //#Todo change from testing to release
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "lason_peter/Solder/0.0.1 (testing) (contact@lasonpeter.net)");
    }

    protected async Task<T?> SendRequestAsync<T>(string relativeUrl)
    {
        var response = await _httpClient.GetAsync(relativeUrl);

        // Handle global errors like Rate Limiting (429) here
        Console.WriteLine(response.Content);

        response.EnsureSuccessStatusCode();
        response.Headers.TryGetValues("X-Ratelimit-Remaining", out var rateLimitRemaining);
        if (rateLimitRemaining != null) RateLimitRemaining = int.Parse(rateLimitRemaining.First());
        return await response.Content.ReadFromJsonAsync<T>();
    }
}