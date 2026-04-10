using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Core.Models;
using Solder.Shared.DTOs.Solder.Auth;
using Solder.Shared.Statics;

namespace Solder.Infrastructure.Persistence.SolderAPI;

/// <summary>
///     Desktop implementation of the <see cref="ISolderAuthService" /> using <see cref="HttpClient" /> and a persistent
///     session store.
/// </summary>
public class SolderAuthService : ISolderAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ISolderAuthSessionStore _sessionStore;
    private bool _initialized;
    private AuthSession? _session;

    public SolderAuthService(HttpClient httpClient, ISolderAuthSessionStore sessionStore)
    {
        _httpClient = httpClient;
        _sessionStore = sessionStore;
    }

    public bool IsAuthenticated => _session is { IsExpired: false };

    public UserInfoCache? GetUserInfo()
    {
        return _session?.UserInfo;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        _session = await _sessionStore.GetSessionAsync();
        _initialized = true;

        if (_session is { IsExpired: true } && !string.IsNullOrWhiteSpace(_session.RefreshToken))
            await TryRefreshAsync();

        SessionChanged?.Invoke();
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Auth.Register, request);
        /*if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response));*/
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Auth.Login, request);
        /*if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response));*/

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (payload is null) return false;
        _session = new AuthSession
        {
            TokenType = payload.TokenType,
            AccessToken = payload.AccessToken,
            RefreshToken = payload.RefreshToken,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn)
        };

        var info = await GetInfoAsync();
        _session.UserInfo = new UserInfoCache(info.Email, info.UserName);
        await _sessionStore.SaveAsync(_session);
        SessionChanged?.Invoke();
        return true;
    }

    public async Task<UserInfoResponse> GetInfoAsync()
    {
        await EnsureReadyAsync();

        var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, SolderUris.Auth.ManageInfo);
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response));

        var info = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
        if (info is null) throw new InvalidOperationException("Could not read user details from API response.");

        return info;
    }

    public async Task<UpdateInfoResponse> UpdateInfoAsync(UpdateInfoRequest requestBody)
    {
        await EnsureReadyAsync();

        var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, SolderUris.Auth.ManageInfo);
        request.Content = JsonContent.Create(requestBody);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response));

        var info = await response.Content.ReadFromJsonAsync<UpdateInfoResponse>();
        if (info is null) throw new InvalidOperationException("Could not read update response from API.");

        if (_session is not null)
        {
            _session.UserInfo = new UserInfoCache(info.Email, _session.UserInfo?.UserName ?? info.Email);
            await _sessionStore.SaveAsync(_session);
            SessionChanged?.Invoke();
        }

        return info;
    }

    public async Task LogoutAsync()
    {
        _session = null;
        await _sessionStore.ClearAsync();
        SessionChanged?.Invoke();
    }

    public bool IsAuthenticatedAsync()
    {
        return _session is { IsExpired: false };
    }

    public SessionChanged? SessionChanged { get; set; }


    public async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string uri)
    {
        await EnsureReadyAsync();
        if (_session is null || string.IsNullOrWhiteSpace(_session.AccessToken))
            throw new InvalidOperationException("You are not authenticated.");

        if (_session.IsExpired)
        {
            var refreshed = await TryRefreshAsync();
            if (!refreshed) throw new InvalidOperationException("Session expired. Please sign in again.");
        }

        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue(_session.TokenType, _session.AccessToken);
        return request;
    }

    private async Task<bool> TryRefreshAsync()
    {
        if (_session is null || string.IsNullOrWhiteSpace(_session.RefreshToken)) return false;

        var response =
            await _httpClient.PostAsJsonAsync(SolderUris.Auth.Refresh, new RefreshRequest(_session.RefreshToken));
        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();
            return false;
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (payload is null)
        {
            await LogoutAsync();
            return false;
        }

        _session.AccessToken = payload.AccessToken;
        _session.RefreshToken = payload.RefreshToken;
        _session.TokenType = payload.TokenType;
        _session.ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn);

        await _sessionStore.SaveAsync(_session);
        SessionChanged?.Invoke();
        return true;
    }

    private async Task EnsureReadyAsync()
    {
        if (!_initialized) await InitializeAsync();
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body)) return $"Request failed with status {(int)response.StatusCode}.";

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var messageElement))
            {
                var message = messageElement.GetString();
                if (!string.IsNullOrWhiteSpace(message)) return message;
            }

            if (doc.RootElement.TryGetProperty("errors", out var errorsElement) &&
                errorsElement.ValueKind == JsonValueKind.Object)
            {
                var allErrors = new List<string>();
                foreach (var property in errorsElement.EnumerateObject())
                {
                    if (property.Value.ValueKind != JsonValueKind.Array) continue;

                    foreach (var item in property.Value.EnumerateArray())
                    {
                        var value = item.GetString();
                        if (!string.IsNullOrWhiteSpace(value)) allErrors.Add(value);
                    }
                }

                if (allErrors.Count > 0) return string.Join(" ", allErrors);
            }
        }
        catch (JsonException)
        {
            // Ignore and fall back to raw payload.
        }

        return body;
    }
}