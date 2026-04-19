using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using TC.Micro.SampleAdmin.Client.Infrastructure.ApiClients;
using TC.Micro.SampleAdmin.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ── MudBlazor ──────────────────────────────────────────────────────────────
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});

// ── 多语言国际化 ──────────────────────────────────────────────────────────
builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");

// ── 认证 ──────────────────────────────────────────────────────────────────
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<WasmAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<WasmAuthStateProvider>());

// ── 管理 API 客户端（Refit）───────────────────────────────────────────────
builder.Services.AddScoped<AuthMessageHandler>();

builder.Services
    .AddRefitClient<IGatewayManagementApi>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["GatewayManagementUrl"]
                               ?? "http://localhost:5101/api"))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services
    .AddRefitClient<IIdentityManagementApi>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["IdentityManagementUrl"]
                               ?? "http://localhost:5202/api"))
    .AddHttpMessageHandler<AuthMessageHandler>();

// ── 应用服务 ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INavMenuService, NavMenuService>();

await builder.Build().RunAsync();
