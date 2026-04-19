using System.Text.Json;
using TC.Micro.Sample.Infrastructure;
#if (IsGrpc)
using TC.Micro.Sample.Api.GrpcServices;
#endif

var builder = WebApplication.CreateBuilder(args);

// ── 1. 基础服务（MicroService 元数据、健康检查基础注册）──────────────────
builder.Services.AddMicroService(builder.Configuration);

// ── 2. 结构化日志（Serilog + OpenTelemetry 输出）──────────────────────────
builder.Host.UseMicroLogging(builder.Configuration);

// ── 3. 分布式链路追踪（OpenTelemetry OTLP）────────────────────────────────
builder.Services.AddMicroTracing(builder.Configuration);

// ── 4. Consul 服务注册 / 发现 ─────────────────────────────────────────────
builder.Services.AddConsulIntegration(builder.Configuration);

// ── 5. JWT 认证 + 网关签名验证（由 TC.Micro.Identity.Client 提供）─────────
builder.Services.AddMicroJwtAuthentication(builder.Configuration);

// ── 6. RBAC 授权（Redis 权限码比对）──────────────────────────────────────
builder.Services.AddMicroAuthorization(builder.Configuration);

// ── 7. 消息队列（RabbitMQ）────────────────────────────────────────────────
builder.Services.AddMicroMessaging(builder.Configuration);

// ── 8. DTM 分布式事务 ─────────────────────────────────────────────────────
builder.Services.AddDtmIntegration(builder.Configuration);

// ── 9. Redis 缓存 ────────────────────────────────────────────────────────
builder.Services.AddMicroCaching(builder.Configuration);

// ── 10. 健康检查（Consul / Redis / RabbitMQ / PostgreSQL）────────────────
builder.Services.AddMicroHealthChecks(builder.Configuration);

#if (IsHttp)
// ── 11. OpenAPI spec 生成（.NET 原生）────────────────────────────────────
builder.Services.AddMicroOpenApi(builder.Configuration);
#endif

// ── 12. EF Core + PostgreSQL ──────────────────────────────────────────────
builder.Services.AddMicroDbContext<AppDbContext>(builder.Configuration);

#if (IsHttp)
// ── 13. 控制器（JSON 统一 snake_case 输出）────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // 对外接口统一使用 snake_case 小写，避免大小写不统一的问题
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        opts.JsonSerializerOptions.DictionaryKeyPolicy  = JsonNamingPolicy.SnakeCaseLower;
    });
#endif

#if (IsGrpc)
// ── 14. gRPC 服务 ─────────────────────────────────────────────────────────
builder.Services.AddGrpc();
#endif

// ── 15. 业务依赖注入（在此处注册 Service 实现）──────────────────────────
builder.Services.AddScoped<TC.Micro.Sample.Application.Services.ISampleService,
                            TC.Micro.Sample.Application.Services.SampleService>();

#if (IsAdminIntegrated)
// ── 16. 集成管理后台（Blazor Server，共用端口）────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
});
builder.Services.AddScoped<TC.Micro.Sample.Admin.Services.IThemeService,
                            TC.Micro.Sample.Admin.Services.ThemeService>();
builder.Services.AddScoped<TC.Micro.Sample.Admin.Services.INavMenuService,
                            TC.Micro.Sample.Admin.Services.NavMenuService>();
builder.Services.AddHttpClient("SampleApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["SampleApiBaseUrl"] ?? "http://localhost:5400");
});
#endif

// ──────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ──────────────────────────────────────────────────────────────────────────

// ── 中间件管道（顺序不可随意调整）────────────────────────────────────────

// 关联 ID 传播（X-Correlation-Id）
app.UseCorrelationId();

// 全局异常处理（统一返回 ServiceResult 格式）
app.UseMicroServiceExceptionHandler();

#if (IsHttp)
// 仅开发/测试环境开放 Scalar API 文档 UI
if (app.Environment.IsDevelopment())
    app.MapMicroOpenApiUi();
#endif

// 认证 & 授权
app.UseAuthentication();
app.UseAuthorization();

// 健康检查端点
app.MapMicroHealthChecks();

#if (IsHttp)
// OpenAPI spec 端点（/openapi/v1.json）
app.MapMicroOpenApi();

// 业务控制器路由
app.MapControllers();
#endif

#if (IsGrpc)
// gRPC 服务路由
app.MapGrpcService<SampleGrpcService>();
#endif

#if (IsAdminIntegrated)
// 集成管理后台 Blazor UI
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<TC.Micro.Sample.Admin.Components.App>()
    .AddInteractiveServerRenderMode();
#endif

app.Run();
