using Grpc.Core;
using TC.Micro.Sample.Application.DTOs.Requests;
using TC.Micro.Sample.Application.Services;

namespace TC.Micro.Sample.Api.GrpcServices;

/// <summary>
/// Sample gRPC 服务实现（供其他微服务内部调用，不对外暴露）
/// 注意：gRPC 服务本身不做认证，由调用方通过 TC.Micro.Identity.Client 验证网关签名。
/// </summary>
public class SampleGrpcService : SampleService.SampleServiceBase
{
    private readonly ISampleService _sampleService;
    private readonly ILogger<SampleGrpcService> _logger;

    public SampleGrpcService(ISampleService sampleService, ILogger<SampleGrpcService> logger)
    {
        _sampleService = sampleService;
        _logger        = logger;
    }

    /// <inheritdoc />
    public override async Task<SampleReply> GetById(
        GetSampleByIdRequest request,
        ServerCallContext context)
    {
        var result = await _sampleService.GetByIdAsync(request.Id, request.TenantId, context.CancellationToken);
        if (result is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Sample {request.Id} not found"));

        return ToReply(result);
    }

    /// <inheritdoc />
    public override async Task<SampleListReply> GetList(
        GetSampleListRequest request,
        ServerCallContext context)
    {
        var paged = await _sampleService.GetListAsync(
            request.Page == 0 ? 1 : request.Page,
            request.PageSize == 0 ? 20 : request.PageSize,
            request.TenantId,
            context.CancellationToken);

        var reply = new SampleListReply
        {
            TotalCount = paged.TotalCount,
            Page       = paged.PageIndex,
            PageSize   = paged.PageSize
        };
        reply.Items.AddRange(paged.Items.Select(ToReply));
        return reply;
    }

    /// <inheritdoc />
    public override async Task<SampleReply> Create(
        CreateSampleRequest request,
        ServerCallContext context)
    {
        // gRPC 内部调用：构造一个简单的 IUserContext-like 参数
        var userContext = new GrpcUserContext(request.TenantId, request.UserId);
        var result = await _sampleService.CreateAsync(
            new Application.DTOs.Requests.CreateSampleRequest
            {
                Name   = request.Name,
                Remark = request.Remark
            },
            userContext,
            context.CancellationToken);

        _logger.LogInformation("gRPC Create: sample_id={Id}, tenant_id={TenantId}",
            result.Id, result.TenantId);

        return ToReply(result);
    }

    // ── 映射辅助 ──────────────────────────────────────────────────────────

    private static SampleReply ToReply(Application.DTOs.Responses.SampleResponse r) => new()
    {
        Id        = r.Id,
        TenantId  = r.TenantId,
        Name      = r.Name,
        Remark    = r.Remark ?? string.Empty,
        CreatedBy = r.CreatedBy ?? string.Empty,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt ?? 0
    };
}

/// <summary>gRPC 调用场景下的轻量 UserContext 实现</summary>
file sealed class GrpcUserContext : TC.Micro.Core.IUserContext
{
    public GrpcUserContext(string tenantId, string userId)
    {
        TenantId = tenantId;
        UserId   = userId;
    }

    public string TenantId { get; }
    public string UserId   { get; }
    public string UserName => string.Empty;
    public IReadOnlyList<string> Roles => Array.Empty<string>();
}
