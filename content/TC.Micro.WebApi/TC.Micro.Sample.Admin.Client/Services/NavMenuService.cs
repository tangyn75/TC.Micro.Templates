namespace TC.Micro.Sample.Admin.Client.Services;

/// <summary>
/// 默认导航菜单实现（服务管理后台）。
/// 业务方可通过注册自己的 <see cref="INavMenuService"/> 实现完全替换：
/// <code>builder.Services.AddScoped&lt;INavMenuService, MyNavMenuService&gt;();</code>
/// </summary>
public class NavMenuService : INavMenuService
{
    private static readonly IReadOnlyList<NavMenuItem> _topLevel =
    [
        new(Title: "服务概览",
            Href: "/",
            Icon: Icons.Material.Outlined.Dashboard),

        new(Title: "运行统计",
            Href: "/management/stats",
            Icon: Icons.Material.Outlined.BarChart),

        new(Title: "运行配置",
            Href: "/management/config",
            Icon: Icons.Material.Outlined.Tune),
    ];

    private static readonly IReadOnlyList<NavMenuGroup> _groups = [];

    /// <inheritdoc />
    public IReadOnlyList<NavMenuItem> GetTopLevelItems() => _topLevel;

    /// <inheritdoc />
    public Task<IReadOnlyList<NavMenuGroup>> GetGroupsAsync(CancellationToken ct = default)
        => Task.FromResult(_groups);
}
