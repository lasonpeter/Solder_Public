using System.Net.Http.Headers;
using System.Net.Http.Json;
using Solder.Core.DTOs.Api.Solder;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Shared.Statics;

namespace Solder.Infrastructure.Persistence.SolderAPI;

public class SolderApiService : ISolderApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;
    private UserInfoCache _userInfoCache;

    public SolderApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        if (_httpClient.BaseAddress == null)
            _httpClient.BaseAddress = new Uri(SolderUris.FallbackBaseUrl);
    }

    public async Task<IEnumerable<ProjectResponse>> GetProjectsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProjectResponse>>(SolderUris.Project.Get) ??
                   new List<ProjectResponse>();
        }
        catch
        {
            return new List<ProjectResponse>();
        }
    }

    public async Task<ProjectResponse?> CreateProjectAsync(CreateProjectRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Create, request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ProjectResponse>();
        return null;
    }

    public async Task<ProjectResponse?> UpdateProjectAsync(UpdateProjectRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Update, request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ProjectResponse>();
        return null;
    }

    public async Task<bool> DeleteProjectAsync(string projectId)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Project.Delete, new { ProjectID = projectId });
        return response.IsSuccessStatusCode;
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    public void Logout()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(SolderUris.Auth.Login, new LoginRequest(email, password));
        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Console.WriteLine(loginResponse.TokenType);
            if (loginResponse != null)
                SetToken(loginResponse.AccessToken);
            return loginResponse;
        }

        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        var userInfo = await GetUserInfoAsync();
        if (userInfo != null)
            _userInfoCache = new UserInfoCache(userInfo.Email, userInfo.UserName);

        return null;
    }

    public Task<bool> IsLoginAsync()
    {
        if (_token is null)
            return Task.FromResult(false);
        try
        {
            /*if (GetUserInfoAsync().Result is null)
                return Task.FromResult(false);*/
        }
        catch (Exception e)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public async Task<bool> RegisterAsync(string email, string username, string password)
    {
        var response =
            await _httpClient.PostAsJsonAsync(SolderUris.Auth.Register, new RegisterRequest(email, username, password));
        Console.WriteLine(response.Content);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserInfoResponse?> GetUserInfoAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserInfoResponse>(SolderUris.Auth.ManageInfo);
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserInfoCache?> GetUserInfoCachedAsync()
    {
        if (_userInfoCache is null)
        {
            var response = await GetUserInfoAsync();
            if (response is null)
                throw new UnauthorizedAccessException();
            _userInfoCache = new UserInfoCache(response.Email, response.UserName);
        }

        return _userInfoCache;
    }
}