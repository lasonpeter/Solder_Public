using System.Text.Json;
using Microsoft.JSInterop;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Core.Models;

namespace Solder.WebAPP.Services;

public sealed class SolderAuthSessionStore : ISolderAuthSessionStore
{
    private const string StorageKey = "solder.auth.session";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IJSRuntime _jsRuntime;

    public SolderAuthSessionStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthSession?> GetSessionAsync()
    {
        var raw = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        return string.IsNullOrWhiteSpace(raw)
            ? null
            : JsonSerializer.Deserialize<AuthSession>(raw, SerializerOptions);
    }

    public Task SaveAsync(AuthSession session)
    {
        var raw = JsonSerializer.Serialize(session, SerializerOptions);
        return _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, raw).AsTask();
    }

    public Task ClearAsync()
    {
        return _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey).AsTask();
    }
}