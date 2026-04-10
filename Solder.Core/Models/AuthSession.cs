using Solder.Shared.DTOs.Solder.Auth;

namespace Solder.Core.Models;

public class AuthSession
{
    public string TokenType { get; set; } = "Bearer";
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public UserInfoCache? UserInfo { get; set; }

    public bool IsExpired => ExpiresAtUtc <= DateTimeOffset.UtcNow;
}