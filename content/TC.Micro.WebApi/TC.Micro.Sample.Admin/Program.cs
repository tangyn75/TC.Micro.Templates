using MudBlazor.Services;
using TC.Micro.Sample.Admin.Services;

// ── Admin 管理进程（独立端口，内部访问）──────────────────────────────────
//
// 运行在独立端口（AdminPort）上，提供：
//   1. 管理 REST API  → /api/management/*（供统一 TC.Micro.Admin 门户调用）
//   2. Blazor Server UI → /（内嵌管理界面，可在配置中禁用）
//
// 生产环境建议通过网络策略限制该端口仅内部可访问。
// ──────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// ── 结构化日志 ────────────────────────────────────────────────────────────
builder.Host.UseMicroLogging(builder.Configuration);

// ── 健康检查 ──────────────────────────────────────────────────────────────
builder.Services.AddMicroHealthChecks(builder.Configuration);

// ── MudBlazor ─────────────────────────────────────────────────────────────
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
});

// ── Blazor Server ─────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── 管理 API 控制器（JSON 统一 snake_case）────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });

// ── 主题服务 ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IThemeService, ThemeService>();

// ── 导航菜单服务（业务方可替换实现）─────────────────────────────────────
builder.Services.AddScoped<INavMenuService, NavMenuService>();

// ── HttpClient（调用本服务 Api 层内部接口获取统计数据）────────────────────
builder.Services.AddHttpClient("SampleApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["SampleApiBaseUrl"] ?? "http://localhost:5400");
});

// ──────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ──────────────────────────────────────────────────────────────────────────

app.UseStaticFiles();
app.UseAntiforgery();

// 管理 REST API（/api/management/*）
app.MapControllers();

// 健康检查端点（/health/ready, /health/live）
app.MapMicroHealthChecks();

// Blazor UI（根路径 /）
app.MapRazorComponents<TC.Micro.Sample.Admin.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
