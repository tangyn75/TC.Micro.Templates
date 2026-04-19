namespace TC.Micro.Sample.Infrastructure.Entities;

/// <summary>
/// Sample 数据实体
/// 业务表必须包含 TenantId 字段实现多租户隔离。
/// 使用软删除（IsDeleted）代替物理删除。
/// </summary>
public class Sample
{
    /// <summary>主键（UUID 字符串，避免暴露业务量）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>租户 ID（多租户隔离必填字段）</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }

    /// <summary>创建人 ID</summary>
    public string? CreatedBy { get; set; }

    /// <summary>创建时间（UTC）</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>最后更新人 ID</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>最后更新时间（UTC，nullable）</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>是否已删除（软删除标志）</summary>
    public bool IsDeleted { get; set; }

    /// <summary>扩展元数据（PostgreSQL JSONB 类型，按需使用）</summary>
    public string Metadata { get; set; } = "{}";
}
