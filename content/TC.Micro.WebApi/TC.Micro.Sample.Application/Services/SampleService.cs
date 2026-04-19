using Microsoft.EntityFrameworkCore;
using TC.Micro.Core;
using TC.Micro.Messaging;
using TC.Micro.Sample.Application.DTOs.Requests;
using TC.Micro.Sample.Application.DTOs.Responses;
using TC.Micro.Sample.Application.Events;
using TC.Micro.Sample.Infrastructure;
using TC.Micro.Sample.Infrastructure.Entities;

namespace TC.Micro.Sample.Application.Services;

/// <summary>Sample 业务服务实现</summary>
public class SampleService : ISampleService
{
    private readonly AppDbContext _db;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SampleService> _logger;

    public SampleService(
        AppDbContext db,
        IMessagePublisher publisher,
        ILogger<SampleService> logger)
    {
        _db        = db;
        _publisher = publisher;
        _logger    = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResponse<SampleResponse>> GetListAsync(
        int pageIndex, int pageSize, string tenantId,
        CancellationToken ct = default)
    {
        var query = _db.Samples
            .Where(s => s.TenantId == tenantId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(s => ToResponse(s))
            .ToListAsync(ct);

        return new PagedResponse<SampleResponse>
        {
            Items      = items,
            TotalCount = total,
            PageIndex  = pageIndex,
            PageSize   = pageSize
        };
    }

    /// <inheritdoc />
    public async Task<SampleResponse?> GetByIdAsync(
        string id, string tenantId,
        CancellationToken ct = default)
    {
        var entity = await _db.Samples
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId && !s.IsDeleted, ct);

        return entity is null ? null : ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<SampleResponse> CreateAsync(
        CreateSampleRequest request, IUserContext userContext,
        CancellationToken ct = default)
    {
        var entity = new Sample
        {
            Id        = Guid.NewGuid().ToString(),
            TenantId  = userContext.TenantId,
            Name      = request.Name,
            Remark    = request.Remark,
            CreatedBy = userContext.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        _db.Samples.Add(entity);
        await _db.SaveChangesAsync(ct);

        // 发布集成事件（最终一致性）
        await _publisher.PublishAsync(new SampleCreatedEvent
        {
            SampleId = entity.Id,
            TenantId = entity.TenantId,
            Name     = entity.Name,
            UserId   = userContext.UserId
        }, ct);

        _logger.LogInformation(
            "Sample created, id={Id}, tenant_id={TenantId}, user_id={UserId}",
            entity.Id, entity.TenantId, userContext.UserId);

        return ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<SampleResponse?> UpdateAsync(
        string id, UpdateSampleRequest request, IUserContext userContext,
        CancellationToken ct = default)
    {
        var entity = await _db.Samples
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == userContext.TenantId && !s.IsDeleted, ct);

        if (entity is null) return null;

        entity.Name      = request.Name;
        entity.Remark    = request.Remark;
        entity.UpdatedBy = userContext.UserId;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        string id, string tenantId,
        CancellationToken ct = default)
    {
        var entity = await _db.Samples
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId && !s.IsDeleted, ct);

        if (entity is null) return false;

        // 软删除
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static SampleResponse ToResponse(Sample s) => new()
    {
        Id        = s.Id,
        TenantId  = s.TenantId,
        Name      = s.Name,
        Remark    = s.Remark,
        CreatedBy = s.CreatedBy,
        CreatedAt = s.CreatedAt.ToUnixTimeMilliseconds(),
        UpdatedAt = s.UpdatedAt?.ToUnixTimeMilliseconds()
    };
}
