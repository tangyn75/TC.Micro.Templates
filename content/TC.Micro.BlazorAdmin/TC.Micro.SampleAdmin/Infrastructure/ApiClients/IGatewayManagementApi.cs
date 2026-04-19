using Refit;
using TC.Micro.SampleAdmin.Models;

namespace TC.Micro.SampleAdmin.Infrastructure.ApiClients;

/// <summary>
/// TC.Micro.Gateway.Admin 管理 REST API 客户端（Refit）
/// 基地址：http://gateway:5101/api
/// </summary>
public interface IGatewayManagementApi
{
    /// <summary>获取已注册服务列表</summary>
    [Get("/services")]
    Task<ApiResponse<PagedResult<GatewayServiceDto>>> GetServicesAsync(
        [Query] int page_index = 1,
        [Query] int page_size  = 20,
        CancellationToken ct   = default);

    /// <summary>获取路由列表</summary>
    [Get("/routes")]
    Task<ApiResponse<PagedResult<GatewayRouteDto>>> GetRoutesAsync(
        [Query] int page_index = 1,
        [Query] int page_size  = 20,
        CancellationToken ct   = default);

    /// <summary>发布当前草稿配置</summary>
    [Post("/publish")]
    Task<IApiResponse> PublishConfigAsync(CancellationToken ct = default);

    /// <summary>回滚到上一个版本</summary>
    [Post("/rollback")]
    Task<IApiResponse> RollbackConfigAsync(CancellationToken ct = default);
}
