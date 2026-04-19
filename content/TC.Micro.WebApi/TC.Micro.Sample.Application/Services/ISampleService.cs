using TC.Micro.Core;
using TC.Micro.Sample.Application.DTOs.Requests;
using TC.Micro.Sample.Application.DTOs.Responses;

namespace TC.Micro.Sample.Application.Services;

/// <summary>Sample 业务服务接口</summary>
public interface ISampleService
{
    /// <summary>分页查询</summary>
    Task<PagedResponse<SampleResponse>> GetListAsync(
        int pageIndex, int pageSize, string tenantId,
        CancellationToken ct = default);

    /// <summary>根据 ID 查询</summary>
    Task<SampleResponse?> GetByIdAsync(
        string id, string tenantId,
        CancellationToken ct = default);

    /// <summary>创建</summary>
    Task<SampleResponse> CreateAsync(
        CreateSampleRequest request, IUserContext userContext,
        CancellationToken ct = default);

    /// <summary>更新</summary>
    Task<SampleResponse?> UpdateAsync(
        string id, UpdateSampleRequest request, IUserContext userContext,
        CancellationToken ct = default);

    /// <summary>删除（软删除）</summary>
    Task<bool> DeleteAsync(
        string id, string tenantId,
        CancellationToken ct = default);
}
