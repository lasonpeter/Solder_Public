using Solder.Shared.DTOs.Solder.Auth;

namespace Solder.Core.Interfaces.Api.Solder;

/// <summary>
///     Delegate for handling session state changes.
/// </summary>
public delegate void SessionChanged();

/// <summary>
///     Provides authentication services including login, registration, and session management.
/// </summary>
public interface ISolderAuthService
{
    /// <summary>
    ///     Event fired when the session state changes (e.g., login, logout, token refresh).
    /// </summary>
    public SessionChanged? SessionChanged { get; set; }

    /// <summary>
    ///     Gets the cached information for the currently authenticated user.
    /// </summary>
    /// <returns>A <see cref="UserInfoCache" /> object if authenticated; otherwise, null.</returns>
    public UserInfoCache? GetUserInfo();

    /// <summary>
    ///     Initializes the authentication service, typically by loading a persisted session.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public Task InitializeAsync();

    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request data.</param>
    /// <returns>True if registration was successful; otherwise, false.</returns>
    public Task<bool> RegisterAsync(RegisterRequest request);

    /// <summary>
    ///     Authenticates a user and starts a new session.
    /// </summary>
    /// <param name="request">The login request data.</param>
    /// <returns>True if login was successful; otherwise, false.</returns>
    public Task<bool> LoginAsync(LoginRequest request);

    /// <summary>
    ///     Retrieves the detailed profile information for the current user from the API.
    /// </summary>
    /// <returns>A <see cref="UserInfoResponse" /> containing user details.</returns>
    public Task<UserInfoResponse> GetInfoAsync();

    /// <summary>
    ///     Updates the current user's profile information.
    /// </summary>
    /// <param name="request">The information update request data.</param>
    /// <returns>An <see cref="UpdateInfoResponse" /> containing the updated details.</returns>
    public Task<UpdateInfoResponse> UpdateInfoAsync(UpdateInfoRequest request);

    /// <summary>
    ///     Ends the current session and clears authentication data.
    /// </summary>
    /// <returns>A task representing the asynchronous logout operation.</returns>
    public Task LogoutAsync();

    /// <summary>
    ///     Checks if there is an active, valid authentication session.
    /// </summary>
    /// <returns>True if authenticated; otherwise, false.</returns>
    public bool IsAuthenticatedAsync();

    /// <summary>
    ///     Creates an <see cref="HttpRequestMessage" /> with the appropriate Authorization header for the current session.
    /// </summary>
    /// <param name="method">The HTTP method to use.</param>
    /// <param name="uri">The target URI.</param>
    /// <returns>A configured <see cref="HttpRequestMessage" />.</returns>
    public Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string uri);
}