using TC.Micro.SampleAdmin.Infrastructure.ApiClients;
using TC.Micro.SampleAdmin.Models;

namespace TC.Micro.SampleAdmin.Services;

/// <summary>
/// JWT 认证服务实现（调用 Identity 管理端口的登录接口）
/// 生产实现中需将 Token 存储到 localStorage（通过 JS Interop）。
/// </summary>
public class AuthService : IAuthService
{
    private readonly IIdentityManagementApi _identityApi;
    private readonly ILogger<AuthService> _logger;
    private string? _token;

    public AuthService(IIdentityManagementApi identityApi, ILogger<AuthService> logger)
    {
        _identityApi = identityApi;
        _logger      = logger;
    }

    /// <inheritdoc />
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

            _token = response.AccessToken;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Login failed for user {Username}", username);
            return false;
        }
    }

    /// <inheritdoc />
    public Task LogoutAsync(CancellationToken ct = default)
    {
        _token = null;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetTokenAsync(CancellationToken ct = default)
        => Task.FromResult(_token);
}
