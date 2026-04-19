namespace TC.Micro.Sample.Application.DTOs.Responses;

/// <summary>Sample 响应 DTO（字段名由 JSON 序列化器自动转换为 snake_case）</summary>
public class SampleResponse
{
    /// <summary>主键 ID</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>租户 ID</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }

    /// <summary>创建人 ID</summary>
    public string? CreatedBy { get; set; }

    /// <summary>创建时间（Unix 毫秒）</summary>
    public long CreatedAt { get; set; }

    /// <summary>最后更新时间（Unix 毫秒，nullable）</summary>
    public long? UpdatedAt { get; set; }
}
