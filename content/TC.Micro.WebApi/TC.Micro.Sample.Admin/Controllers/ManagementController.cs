using Microsoft.AspNetCore.Mvc;

namespace TC.Micro.Sample.Admin.Controllers;

/// <summary>
/// 管理 REST API 端点（供 TC.Micro.Admin 统一门户调用）
/// 所有端点路径前缀：/api/management/
/// </summary>
[ApiController]
[Route("api/management")]
public class ManagementController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ManagementController> _logger;

    public ManagementController(
        IHttpClientFactory httpClientFactory,
        ILogger<ManagementController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger            = logger;
    }

    /// <summary>服务概览信息</summary>
    [HttpGet("info")]
    [ProducesResponseType(typeof(ServiceInfoResponse), StatusCodes.Status200OK)]
    public IActionResult GetInfo() =>
        Ok(new ServiceInfoResponse
        {
            ServiceName = "TC.Micro.Sample",
            Version     = typeof(ManagementController).Assembly
                             .GetName().Version?.ToString() ?? "1.0.0",
            StartedAt   = s_startedAt
        });

    /// <summary>实时统计摘要（从 Api 层内部接口拉取）</summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ServiceStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        // TODO: 通过 SampleApi HttpClient 调用 Api 层 /internal/stats 接口
        // var client = _httpClientFactory.CreateClient("SampleApi");
        // var stats = await client.GetFromJsonAsync<ServiceStatsResponse>("/internal/stats", ct);
        await Task.CompletedTask;
        return Ok(new ServiceStatsResponse { TotalRecords = 0 });
    }

    // ── 应用启动时间（静态记录）────────────────────────────────────────────
    private static readonly DateTimeOffset s_startedAt = DateTimeOffset.UtcNow;
}

// ── 响应 DTO ──────────────────────────────────────────────────────────────

public record ServiceInfoResponse
{
    public string ServiceName { get; init; } = string.Empty;
    public string Version     { get; init; } = string.Empty;
    public DateTimeOffset StartedAt { get; init; }
}

public record ServiceStatsResponse
{
    public long TotalRecords { get; init; }
}
