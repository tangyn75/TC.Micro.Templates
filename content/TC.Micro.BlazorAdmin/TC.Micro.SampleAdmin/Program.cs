#if (IsServer)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;
#endif
#if (IsWasm)
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endif
#if (IsAuto)
using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;
#endif
using MudBlazor.Services;
using Refit;
using TC.Micro.SampleAdmin.Infrastructure.ApiClients;
using TC.Micro.SampleAdmin.Services;

#if (IsServer)
// ── 配置 Serilog ───────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
#endif
#if (IsWasm)
var builder = WebAssemblyHostBuilder.CreateDefault(args);
#endif
#if (IsServer)
var builder = WebApplication.CreateBuilder(args);
#endif
#if (IsAuto)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, config) =>
    config.ReadFrom.Configuration(ctx.Configuration)
          .ReadFrom.Services(services)
          .Enrich.FromLogContext());
#endif

    // ── Blazor 渲染模式 ──────────────────────────────────────────────────────
#if (IsServer)
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
#endif
#if (IsWasm)
    builder.Services.AddRazorComponents()
        .AddInteractiveWebAssemblyComponents();
#endif
#if (IsAuto)
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents()
        .AddAdditionalAssemblies(typeof(TC.Micro.SampleAdmin.Client._Imports).Assembly);
#endif

    // ── MudBlazor（Material Design 3 UI 库）────────────────────────────────
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

    // ── 多语言国际化（zh-Hans / en-US）────────────────────────────────────
    builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");
#if (IsServer || IsAuto)
    builder.Services.Configure<RequestLocalizationOptions>(opts =>
    {
        var cultures = new[] { "zh-Hans", "en-US" };
        opts.DefaultRequestCulture = new RequestCulture("zh-Hans");
        opts.SupportedCultures    = cultures.Select(c => new CultureInfo(c)).ToList();
        opts.SupportedUICultures  = cultures.Select(c => new CultureInfo(c)).ToList();
    });
#endif

    // ── 认证 ──────────────────────────────────────────────────────────────
#if (IsServer)
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.Authority = builder.Configuration["Jwt:Authority"];
            opts.RequireHttpsMetadata = false;
        });
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();
#endif
#if (IsWasm)
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<WasmAuthStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
        sp.GetRequiredService<WasmAuthStateProvider>());
#endif
#if (IsAuto)
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.Authority = builder.Configuration["Jwt:Authority"];
            opts.RequireHttpsMetadata = false;
        });
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();
#endif

    // ── 管理 API 客户端（Refit）───────────────────────────────────────────
#if (IsWasm)
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
#endif
#if (IsServer || IsAuto)
    builder.Services
        .AddRefitClient<IGatewayManagementApi>()
        .ConfigureHttpClient(c =>
            c.BaseAddress = new Uri(builder.Configuration["GatewayManagementUrl"]
                                   ?? "http://localhost:5101/api"));

    builder.Services
        .AddRefitClient<IIdentityManagementApi>()
        .ConfigureHttpClient(c =>
            c.BaseAddress = new Uri(builder.Configuration["IdentityManagementUrl"]
                                   ?? "http://localhost:5202/api"));
#endif

    // ── 应用服务 ──────────────────────────────────────────────────────────
    builder.Services.AddScoped<IThemeService, ThemeService>();
    builder.Services.AddScoped<ILayoutService, LayoutService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<INavMenuService, NavMenuService>();

    // ──────────────────────────────────────────────────────────────────────
#if (IsWasm)
await builder.Build().RunAsync();
#endif
#if (IsServer || IsAuto)
    var app = builder.Build();
    // ──────────────────────────────────────────────────────────────────────

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }

    app.UseRequestLocalization();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapRazorComponents<TC.Micro.SampleAdmin.Components.App>()
#if (IsServer)
        .AddInteractiveServerRenderMode();
#endif
#if (IsAuto)
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode();
#endif

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
#endif
