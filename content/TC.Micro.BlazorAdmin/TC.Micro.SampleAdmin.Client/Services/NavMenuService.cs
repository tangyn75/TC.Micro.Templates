namespace TC.Micro.SampleAdmin.Client.Services;

/// <summary>
/// 默认导航菜单实现。
/// 返回 Gateway 和 Identity 管理菜单作为初始示例；
/// 业务方可通过注册自己的 <see cref="INavMenuService"/> 实现完全替换。
/// </summary>
public class NavMenuService : INavMenuService
{
    private static readonly IReadOnlyList<NavMenuItem> _topLevel =
    [
        new(Title: "仪表盘",
            Href: "/",
            Icon: Icons.Material.Outlined.Dashboard)
    ];

    private static readonly IReadOnlyList<NavMenuGroup> _groups =
    [
        new(Title: "Gateway",
            Icon: Icons.Material.Outlined.Hub,
            Items:
            [
                new("服务列表",  "/gateway/services", Icons.Material.Outlined.Dns),
                new("路由配置",  "/gateway/routes",   Icons.Material.Outlined.AltRoute),
                new("插件管理",  "/gateway/plugins",  Icons.Material.Outlined.Extension),
            ]),

        new(Title: "Identity",
            Icon: Icons.Material.Outlined.Security,
            Items:
            [
                new("用户管理",  "/identity/users",    Icons.Material.Outlined.People),
                new("角色权限",  "/identity/roles",    Icons.Material.Outlined.VerifiedUser),
                new("租户管理",  "/identity/tenants",  Icons.Material.Outlined.Business),
                new("ABAC 策略", "/identity/policies", Icons.Material.Outlined.Policy),
            ]),
    ];

    /// <inheritdoc />
    public IReadOnlyList<NavMenuItem> GetTopLevelItems() => _topLevel;

    /// <inheritdoc />
    public Task<IReadOnlyList<NavMenuGroup>> GetGroupsAsync(CancellationToken ct = default)
        => Task.FromResult(_groups);
}
