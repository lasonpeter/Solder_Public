namespace Solder.Core.DTOs.Api.Solder;

public record LoginRequest(
    string Email,
    string Password,
    string? TwoFactorCode = null,
    string? TwoFactorRecoveryCode = null);

public record UserInfoCache(string Email, string UserName);

public record LoginResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);

public record RegisterRequest(string Email, string UserName, string Password);

public record UserInfoResponse(string Email, bool IsEmailConfirmed, string UserName);

// PROJECT MODELS
public record ProjectResponse(Guid ProjectId, string ProjectName, string? ProjectDescription = null);

public class CreateProjectRequest
{
    public string ProjectName { get; set; } = default!;
    public string? ProjectDescription { get; set; }
}

public class CreateProjectResponse
{
    public string ProjectName { get; set; } = default!;
    public string? ProjectDescription { get; set; }
    public Guid ProjectId { get; set; } = default!;
}

public class DeleteProjectRequest
{
    public Guid ProjectID { get; set; } = default!;
}

public class DeleteProjectResponse
{
    public Guid ProjectId { get; set; }
}

public class UpdateProjectRequest
{
    public Guid ProjectId { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string? ProjectDescription { get; set; }
}

public class UpdateProjectResponse
{
    public Guid ProjectId { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string? ProjectDescription { get; set; }
}