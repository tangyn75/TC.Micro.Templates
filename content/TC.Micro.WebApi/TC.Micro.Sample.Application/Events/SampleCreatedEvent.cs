using TC.Micro.Messaging;

namespace TC.Micro.Sample.Application.Events;

/// <summary>Sample 创建事件（集成事件，发布到 MQ）</summary>
public class SampleCreatedEvent : IntegrationEvent
{
    /// <summary>创建的 Sample ID</summary>
    public string SampleId { get; set; } = string.Empty;

    /// <summary>租户 ID（多租户隔离必须携带）</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>触发操作的用户 ID</summary>
    public string UserId { get; set; } = string.Empty;
}
