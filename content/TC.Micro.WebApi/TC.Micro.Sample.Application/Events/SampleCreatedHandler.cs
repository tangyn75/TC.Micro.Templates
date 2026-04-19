using Microsoft.Extensions.Logging;
using TC.Micro.Messaging;

namespace TC.Micro.Sample.Application.Events;

/// <summary>
/// SampleCreatedEvent 消费者
/// 在此实现跨服务最终一致性逻辑（如通知服务、统计服务等）
/// </summary>
public class SampleCreatedHandler : IMessageConsumer<SampleCreatedEvent>
{
    private readonly ILogger<SampleCreatedHandler> _logger;

    public SampleCreatedHandler(ILogger<SampleCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(SampleCreatedEvent message, CancellationToken ct)
    {
        _logger.LogInformation(
            "Handling SampleCreatedEvent: sample_id={SampleId}, tenant_id={TenantId}, name={Name}",
            message.SampleId, message.TenantId, message.Name);

        // TODO: 在此处实现跨服务联动逻辑
        // 注意：始终使用 message.TenantId 保持多租户隔离

        return Task.CompletedTask;
    }
}
