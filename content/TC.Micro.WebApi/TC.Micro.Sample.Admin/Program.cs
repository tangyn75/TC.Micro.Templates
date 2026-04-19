#if (IsAdminServer || IsAdminAuto)
using Serilog;
#endif
using MudBlazor.Services;
using TC.Micro.Sample.Admin.Services;

#if (IsAdminWasm)
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endif

#if (IsAdminServer || IsAdminAuto)
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseMicroLogging(builder.Configuration);
builder.Services.AddMicroHealthChecks(builder.Configuration);
#endif
#if (IsAdminWasm)
var builder = WebAssemblyHostBuilder.CreateDefault(args);
#endif

// ── MudBlazor ─────────────────────────────────────────────────────────────
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
});

// ── Blazor 渲染模式 ──────────────────────────────────────────────────────
#if (IsAdminServer)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
#endif
#if (IsAdminWasm)
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
#endif
#if (IsAdminAuto)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAdditionalAssemblies(typeof(TC.Micro.Sample.Admin.Client._Imports).Assembly);
#endif

#if (IsAdminServer || IsAdminAuto)
// ── 管理 API 控制器（JSON 统一 snake_case）────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });
#endif

// ── 主题 + 导航菜单 ──────────────────────────────────────────────────────
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<INavMenuService, NavMenuService>();

// ── HttpClient（调用 Api 层内部接口）───────────────────────────────────────
builder.Services.AddHttpClient("SampleApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["SampleApiBaseUrl"] ?? "http://localhost:5400");
});

// ──────────────────────────────────────────────────────────────────────────
#if (IsAdminWasm)
await builder.Build().RunAsync();
#endif
#if (IsAdminServer || IsAdminAuto)
var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();
app.MapMicroHealthChecks();

app.MapRazorComponents<TC.Micro.Sample.Admin.Components.App>()
#if (IsAdminServer)
    .AddInteractiveServerRenderMode();
#endif
#if (IsAdminAuto)
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();
#endif

app.Run();
#endif
