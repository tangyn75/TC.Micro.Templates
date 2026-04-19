using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using Refit;
using Serilog;
using System.Globalization;
using TC.Micro.SampleAdmin.Infrastructure.ApiClients;
using TC.Micro.SampleAdmin.Services;

// ── 配置 Serilog ───────────────────────────────────────────────────────────
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

    // ── Blazor Server ──────────────────────────────────────────────────────
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

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
    builder.Services.Configure<RequestLocalizationOptions>(opts =>
    {
        var cultures = new[] { "zh-Hans", "en-US" };
        opts.DefaultRequestCulture = new RequestCulture("zh-Hans");
        opts.SupportedCultures    = cultures.Select(c => new CultureInfo(c)).ToList();
        opts.SupportedUICultures  = cultures.Select(c => new CultureInfo(c)).ToList();
    });

    // ── JWT 认证（使用 Identity 管理端口颁发的 Token）────────────────────
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.Authority = builder.Configuration["Jwt:Authority"];
            opts.RequireHttpsMetadata = false;
        });
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();

    // ── 管理 API 客户端（Refit）───────────────────────────────────────────
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

    // ── 应用服务 ──────────────────────────────────────────────────────────
    builder.Services.AddScoped<IThemeService, ThemeService>();
    builder.Services.AddScoped<ILayoutService, LayoutService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<INavMenuService, NavMenuService>();

    // ──────────────────────────────────────────────────────────────────────
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
        .AddInteractiveServerRenderMode();

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
