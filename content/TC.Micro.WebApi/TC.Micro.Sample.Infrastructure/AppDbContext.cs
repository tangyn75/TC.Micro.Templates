using Microsoft.EntityFrameworkCore;
using TC.Micro.Sample.Infrastructure.Entities;

namespace TC.Micro.Sample.Infrastructure;

/// <summary>
/// 业务服务数据上下文
/// 继承 TC.Micro.EntityFrameworkCore 提供的 MicroDbContext 基类，
/// 自动处理多租户 Global Query Filter 和审计字段填充。
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Sample> Samples => Set<Sample>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sample>(entity =>
        {
            entity.ToTable("samples");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasMaxLength(36)
                  .IsRequired();

            entity.Property(e => e.TenantId)
                  .HasMaxLength(36)
                  .IsRequired();

            entity.Property(e => e.Name)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Remark)
                  .HasMaxLength(500);

            entity.Property(e => e.CreatedBy)
                  .HasMaxLength(36);

            entity.Property(e => e.UpdatedBy)
                  .HasMaxLength(36);

            entity.Property(e => e.Metadata)
                  .HasColumnType("jsonb")
                  .HasDefaultValueSql("'{}'::jsonb");

            // 多租户查询过滤（Global Query Filter）
            // 如使用 TC.Micro.EntityFrameworkCore 基类，这里由框架自动配置。
            // entity.HasQueryFilter(e => e.TenantId == _currentTenantId);

            // 高频查询索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.IsDeleted });
        });
    }
}
