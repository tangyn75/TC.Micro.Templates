using TC.Micro.SampleAdmin.Client.Infrastructure.ApiClients;
using TC.Micro.SampleAdmin.Client.Models;

namespace TC.Micro.SampleAdmin.Client.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityManagementApi _identityApi;
    private readonly ILogger<AuthService> _logger;
    private readonly WasmAuthStateProvider _authStateProvider;

    public AuthService(
        IIdentityManagementApi identityApi,
        ILogger<AuthService> logger,
        WasmAuthStateProvider authStateProvider)
    {
        _identityApi       = identityApi;
        _logger            = logger;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var response = await _identityApi.LoginAsync(new LoginRequest
            {
                Username = username,
                Password = password
            }, ct);

            if (response?.AccessToken is null) return false;

            _authStateProvider.SetToken(response.AccessToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Login failed for user {Username}", username);
            return false;
        }
    }

    public Task LogoutAsync(CancellationToken ct = default)
    {
        _authStateProvider.SetToken(null);
        return Task.CompletedTask;
    }

    public Task<string?> GetTokenAsync(CancellationToken ct = default)
        => Task.FromResult(_authStateProvider.GetToken());
}
