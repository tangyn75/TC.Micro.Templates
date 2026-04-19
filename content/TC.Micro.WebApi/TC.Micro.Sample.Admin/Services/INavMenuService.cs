namespace TC.Micro.Sample.Admin.Services;

// ── 导航菜单模型 ────────────────────────────────────────────────────────────

/// <summary>导航菜单叶子项（对应一条 MudNavLink）</summary>
public sealed record NavMenuItem(
    string Title,
    string Href,
    string Icon,
    string? Badge = null);

/// <summary>导航菜单分组（对应一个 MudNavGroup，可包含多个叶子项）</summary>
public sealed record NavMenuGroup(
    string Title,
    string Icon,
    IReadOnlyList<NavMenuItem> Items);

// ── 导航菜单服务接口 ────────────────────────────────────────────────────────

/// <summary>
/// 动态导航菜单服务。
/// 业务方在 Program.cs 中注册自己的实现即可替换默认菜单：
/// <code>builder.Services.AddScoped&lt;INavMenuService, MyNavMenuService&gt;();</code>
/// </summary>
public interface INavMenuService
{
    /// <summary>返回顶级直接导航链接（不分组）</summary>
    IReadOnlyList<NavMenuItem> GetTopLevelItems();

    /// <summary>返回菜单分组列表</summary>
    Task<IReadOnlyList<NavMenuGroup>> GetGroupsAsync(CancellationToken ct = default);
}
