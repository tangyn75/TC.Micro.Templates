namespace TC.Micro.SampleAdmin.Client.Services;

/// <summary>认证服务接口（JWT 登录 / 登出）</summary>
public interface IAuthService
{
    /// <summary>登录，成功返回 true</summary>
    Task<bool> LoginAsync(string username, string password, CancellationToken ct = default);

    /// <summary>登出（清除本地 Token）</summary>
    Task LogoutAsync(CancellationToken ct = default);

    /// <summary>获取当前有效 JWT（用于注入请求头）</summary>
    Task<string?> GetTokenAsync(CancellationToken ct = default);
}
