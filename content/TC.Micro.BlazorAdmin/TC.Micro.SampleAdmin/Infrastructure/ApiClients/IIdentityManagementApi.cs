using Refit;
using TC.Micro.SampleAdmin.Models;

namespace TC.Micro.SampleAdmin.Infrastructure.ApiClients;

/// <summary>
/// TC.Micro.Identity.Admin 管理 REST API 客户端（Refit）
/// 基地址：http://identity:5202/api
/// </summary>
public interface IIdentityManagementApi
{
    /// <summary>管理员登录（获取 JWT）</summary>
    [Post("/auth/login")]
    Task<LoginResponse?> LoginAsync([Body] LoginRequest request, CancellationToken ct = default);

    /// <summary>获取用户列表</summary>
    [Get("/users")]
    Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(
        [Query] int page_index    = 1,
        [Query] int page_size     = 20,
        [Query] string? tenant_id = null,
        CancellationToken ct      = default);

    /// <summary>获取角色列表</summary>
    [Get("/roles")]
    Task<ApiResponse<PagedResult<RoleDto>>> GetRolesAsync(
        [Query] int page_index = 1,
        [Query] int page_size  = 20,
        CancellationToken ct   = default);

    /// <summary>获取租户列表</summary>
    [Get("/tenants")]
    Task<ApiResponse<PagedResult<TenantDto>>> GetTenantsAsync(
        [Query] int page_index = 1,
        [Query] int page_size  = 20,
        CancellationToken ct   = default);

    /// <summary>获取 ABAC 策略列表</summary>
    [Get("/abac-policies")]
    Task<ApiResponse<PagedResult<AbacPolicyDto>>> GetPoliciesAsync(
        [Query] int page_index = 1,
        [Query] int page_size  = 20,
        CancellationToken ct   = default);
}
