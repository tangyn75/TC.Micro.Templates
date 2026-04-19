using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TC.Micro.Sample.Admin.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ── MudBlazor ─────────────────────────────────────────────────────────────
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
});

// ── Blazor 组件 ──────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// ── 主题 + 导航菜单 ──────────────────────────────────────────────────────
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<INavMenuService, NavMenuService>();

// ── HttpClient（调用 Api 层内部接口）───────────────────────────────────────
builder.Services.AddHttpClient("SampleApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["SampleApiBaseUrl"] ?? "http://localhost:5400");
});

await builder.Build().RunAsync();