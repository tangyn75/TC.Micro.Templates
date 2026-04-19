# TC.Micro 业务开发规范

> 版本：1.4 | 日期：2026-04-19
> 变更：v1.4 Identity 升级为双端口双进程，与 Gateway 保持一致：5200 业务 API，5202 管理端口（REST API + Blazor UI，独立进程）

---

## 1. 新建业务服务流程

### 1.1 选择模板

| 场景 | 模板 | 说明 |
|------|------|------|
| 对外提供 RESTful API | TC.Micro.Template.WebApi | 标准业务服务（三层项目） |
| 纯内部服务间调用 | TC.Micro.Template.Grpc | 高性能，仅供其他服务调用 |
| 混合模式 | WebApi + gRPC 双端点 | 同时暴露 HTTP 和 gRPC |

### 1.2 创建步骤

```bash
# 1. 使用模板一次性创建三个项目
dotnet new tcmicro-webapi -n TC.Micro.YourService -o TC.Micro.YourService

# 模板会生成：
# TC.Micro.YourService.Api/
# TC.Micro.YourService.Application/
# TC.Micro.YourService.Infrastructure/

# 2. 将三个项目添加到解决方案
dotnet sln add TC.Micro.YourService.Api/TC.Micro.YourService.Api.csproj
dotnet sln add TC.Micro.YourService.Application/TC.Micro.YourService.Application.csproj
dotnet sln add TC.Micro.YourService.Infrastructure/TC.Micro.YourService.Infrastructure.csproj

# 3. 修改 Api/appsettings.json
# - MicroService.ServiceName → your-service
# - Consul.ServiceName → your-service
# - Consul.ServicePort → 分配的端口
# - OpenApi.Title → Your Service API

# 4. 在网关添加路由
# 编辑 TC.Micro.Gateway/appsettings.json
# 在 ReverseProxy:Routes 添加 your-service 路由规则
# 同时配置路由所需的 permission code
```

### 1.3 注册新服务到 Identity

```bash
# 新服务需要在 Identity 中注册：
# 1. 在 sys_resource 表中插入服务资源记录
# 2. 定义服务所需的 permission code（如 order:access, order:create）
# 3. 将 permission 分配给相应角色
# 4. 如需 ABAC 策略，在 sys_abac_policy 中定义规则
```

### 1.4 API 文档

新服务**不需要独立部署 UI**。`TC.Micro.OpenApi` 包自动配置 OpenAPI spec 生成，网关通过 Scalar 聚合所有服务文档。

```
统一文档入口：http://gateway:5100/scalar
→ 自动按服务分组展示所有 API
→ 支持 Try It Out 在线调试
→ 仅开发/测试环境开放
```

---

## 2. 端口分配规范

| 端口范围 | 用途 | 示例 |
|---------|------|------|
| 5100 | YARP 网关（流量入口） | TC.Micro.Gateway.Api |
| 5101 | YARP 网关管理端口（REST API + 内置 UI） | TC.Micro.Gateway.Admin |
| 5200 | Identity HTTP 业务 API（JWT 签发/校验，关键路径） | TC.Micro.Identity.Api |
| 5201 | Identity gRPC | TC.Micro.Identity |
| 5202 | Identity 管理端口（REST API + 内置 UI，内网专用） | TC.Micro.Identity.Admin |
| 5400-5599 | 业务服务 | order, inventory, payment... |
| 5600-5699 | gRPC 业务服务 | 纯 gRPC 内部服务 |

**规则**：新服务在 5400 起递增分配，到架构组登记。

---

## 3. 代码规范

### 3.1 项目结构

TC.Micro 有两种项目结构：**业务服务（三项目）** 和 **基础设施服务（四项目）**。

---

#### 3.1.1 业务服务：三项目结构

普通业务服务（order、inventory、payment 等）**不需要内置管理 UI**，采用三项目结构。  
**项目引用关系**：`Api → Application → Infrastructure`

```
TC.Micro.{ServiceName}.Api/              # ① API 层（ASP.NET Core Web API）
├── Controllers/                         # HTTP / gRPC 入口，薄层，只做参数校验和调用 Service
├── Middlewares/                         # 自定义中间件（可选）
├── Program.cs                           # 入口、DI 注册
└── appsettings.json

TC.Micro.{ServiceName}.Application/      # ② 业务层（Class Library）
├── Services/
│   ├── I{Name}Service.cs                # 业务接口
│   └── {Name}Service.cs                 # 业务实现（直接使用 AppDbContext，无 Repository）
├── DTOs/
│   ├── Requests/                        # 请求 DTO
│   └── Responses/                       # 响应 DTO
├── Validators/                          # FluentValidation 验证器（与 DTO 同层）
│   └── {Name}Validator.cs
└── Events/                              # 集成事件定义 + 处理器
    ├── {Action}{Resource}Event.cs
    └── {Action}{Resource}Handler.cs

TC.Micro.{ServiceName}.Infrastructure/   # ③ 数据层（Class Library）
├── AppDbContext.cs                      # EF Core DbContext
├── Entities/                            # 数据库实体
│   └── {Name}.cs
├── Migrations/                          # EF Core 迁移文件
└── Clients/                             # 外部服务 HTTP/gRPC 客户端（可选）
    └── {Service}Client.cs
```

---

#### 3.1.2 基础设施服务：四项目结构

Gateway、Identity 等需要内置管理 UI 的基础设施服务采用**四项目结构**，Admin 项目独立，代码不混合。  
**项目引用关系**：`Api → Application → Infrastructure`，`Admin → Application`（Admin 直接调用业务层，不经过 Api 层）

```
TC.Micro.{ServiceName}.Api/              # ① API 层（ASP.NET Core Web API）
TC.Micro.{ServiceName}.Application/      # ② 业务层（Class Library）
TC.Micro.{ServiceName}.Infrastructure/   # ③ 数据层（Class Library）

TC.Micro.{ServiceName}.Admin/            # ④ 内置管理 UI（Blazor Server）
├── Pages/                               # Blazor 页面组件
├── Shared/                              # 共享布局/导航
├── Program.cs                           # 独立入口（含 MudBlazor 注册）
└── appsettings.json                     # 可配置关闭：Admin:Enabled: false
```

**Gateway 双端口部署示例：**
```
流量入口：http://gateway:5100        ← TC.Micro.Gateway.Api
管理入口：http://gateway:5101        ← TC.Micro.Gateway.Admin（REST API + Blazor UI）
         http://gateway:5101/api     ← 管理 REST API（供 TC.Micro.Admin 或脚本调用）
         http://gateway:5101/        ← 内置 Blazor 管理 UI（可关闭）
```

**Identity 双端口部署示例：**
```
业务入口：http://identity:5200        ← TC.Micro.Identity.Api（JWT 签发/校验，高频关键路径，可水平扩展）
          http://identity:5200/auth/... ← 认证端点（token/refresh/revoke）
          http://identity:5200/api/...  ← 其他业务 API
gRPC    ：http://identity:5201        ← gRPC 权限校验接口
管理入口：http://identity:5202        ← TC.Micro.Identity.Admin（独立进程，内网专用）
          http://identity:5202/api     ← 管理 REST API（供 TC.Micro.Admin 或脚本调用）
          http://identity:5202/        ← 内置 Blazor 管理 UI（可关闭）
```

**内置 UI 关闭方式（`appsettings.json`）：**
```json
{
  "Admin": {
    "Enabled": false
  }
}
```
关闭后管理 REST API 仍然可用，供 TC.Micro.Admin 统一管理后台调用。

**分层职责说明：**

| 层 | 项目 | 依赖 | 禁止 |
|----|------|------|------|
| Api | `{Service}.Api` | Application、Infrastructure（DI 注册用） | 业务逻辑、直接操作 DB |
| Application | `{Service}.Application` | Infrastructure（使用 AppDbContext） | 直接引用 Api |
| Infrastructure | `{Service}.Infrastructure` | EF Core、Npgsql、外部 SDK | 引用 Application、Api |

> **为什么 Application 直接使用 AppDbContext？**
> 微服务体量小，引入 Repository 只是增加一层无意义的转发。Service 直接注入 `AppDbContext` 使用 LINQ/EF Core，足够清晰且可测试（DbContext 可 Mock 或使用 InMemory Provider）。

### 3.2 命名规范

#### 3.2.1 C# 代码命名

| 类型 | 规范 | 示例 |
|------|------|------|
| Controller | {Resource}Controller | OrderController |
| Service 接口 | I{Name}Service | IOrderService |
| Service 实现 | {Name}Service | OrderService |
| Entity | {Name}（无后缀） | Order |
| 请求 DTO | {Action}{Resource}Request | CreateOrderRequest |
| 响应 DTO | {Resource}Response / {Resource}Dto | OrderResponse |
| 集成事件 | {Action}{Resource}Event | OrderCreatedEvent |
| 事件处理器 | {Event}Handler | OrderCreatedHandler |

#### 3.2.2 API 对外接口命名（snake_case）

**所有对外接口统一使用小写蛇形命名（snake_case），避免大小写混用问题。**

| 范围 | 规范 | 示例 |
|------|------|------|
| JSON 请求字段名 | snake_case | `user_id`, `created_at`, `page_index` |
| JSON 响应字段名 | snake_case | `total_count`, `has_next_page`, `order_no` |
| URL 路径段 | kebab-case | `/api/orders/{order_id}`, `/api/user-groups` |
| Query 参数名 | snake_case | `?page_index=1&page_size=20&start_date=...` |
| HTTP Header 名 | HTTP 标准首字母大写 + 连字符 | `X-User-Id`, `X-Tenant-Id`, `X-Correlation-Id` |

> C# DTO 属性名仍使用 PascalCase（如 `UserId`、`TotalCount`），由 JSON 序列化器统一转换为 snake_case 输出，**不在 DTO 上手动加 `[JsonPropertyName]`**。  
> 配置方式见 §3.3。

### 3.3 API 响应格式

所有 API 必须返回 `ServiceResult<T>` 统一格式：

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "order_id": "abc-123",
    "total_amount": 99.00,
    "created_at": 1713244800000
  },
  "timestamp": 1713244800000
}
```

| code | 含义 |
|------|------|
| 0 | 成功 |
| -1 | 通用业务错误 |
| 1001 | 参数校验失败 |
| 1002 | 资源不存在 |
| 1003 | 权限不足 |
| 2001 | 数据库错误 |
| 2002 | 外部服务调用失败 |

**JSON 序列化配置（统一在模板 `Program.cs` 中设置，不要在各 DTO 上单独设置 `[JsonPropertyName]`）：**

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 对外接口统一使用 snake_case 小写，避免大小写不统一
        options.JsonSerializerOptions.PropertyNamingPolicy =
            JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy =
            JsonNamingPolicy.SnakeCaseLower;
    });
```

对于最小 API（Minimal API）则在 `AddMicroOpenApi()` 内统一配置，业务代码无需关心。

### 3.4 异常处理

```csharp
// 业务异常：已知错误，抛出 BusinessException
throw new BusinessException("订单不存在", code: 1002);

// 全局异常中间件会自动捕获并返回：
// HTTP 400 + { "code": 1002, "message": "订单不存在" }
```

---

## 4. 身份与鉴权

### 4.1 网关签名验证

业务服务通过 `TC.Micro.Identity.Client` 包自动验证网关注入的签名 Header，**业务代码无需手动处理**。

网关注入的 Header：

| Header | 来源 | 说明 |
|--------|------|------|
| X-User-Id | JWT Claim "sub" | 用户 ID |
| X-Tenant-Id | JWT Claim "tenant_id" | 租户 ID |
| X-User-Type | JWT Claim "type" | "user" 或 "service_account" |
| X-Correlation-Id | 生成或透传 | 关联 ID |
| X-Gateway-Signature | HMAC-SHA256 | 网关签名 |
| X-Gateway-Timestamp | 当前时间 | 防重放 |
| X-Gateway-Nonce | 随机 GUID | 防重放 |

验证流程（由框架中间件自动执行）：
1. 清除客户端伪造的 X-User-Id / X-Tenant-Id
2. 检查 Timestamp 是否在 30 秒内
3. Redis SETNX 检查 Nonce 防重放
4. HMAC-SHA256 签名比对
5. 从 Redis 读取用户详情，构建 IUserContext

### 4.2 IUserContext 使用

```csharp
// 在 Controller 或 Service 中直接注入
public class OrderController : ControllerBase
{
    private readonly IUserContext _userContext;

    public OrderController(IUserContext userContext)
    {
        _userContext = userContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        // 获取当前用户信息
        var userId = _userContext.UserId;        // 用户 ID
        var tenantId = _userContext.TenantId;    // 租户 ID
        var userType = _userContext.UserType;    // "user" | "service_account"
        var roles = _userContext.Roles;          // 角色列表
        var orgs = _userContext.Organizations;   // 所属组织
        var tags = _userContext.Tags;            // 用户标签

        // 自动关联租户
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId,
            CreatedBy = userId,
            // ...
        };
    }
}
```

### 4.3 RBAC 权限控制

网关侧已完成粗粒度 RBAC 校验（路由级别），业务服务无需重复。

如需在业务代码中做细粒度判断：

```csharp
// 基于角色的判断
if (!_userContext.Roles.Contains("order_manager"))
{
    throw new BusinessException("需要订单管理员角色", code: 1003);
}

// 基于权限代码的判断（通过 Identity gRPC 调用）
var hasPermission = await _permissionClient.CheckPermissionAsync(
    new CheckPermissionRequest
    {
        UserId = _userContext.UserId,
        TenantId = _userContext.TenantId,
        PermissionCode = "order:approve",
        ResourceAttributes = { ["amount"] = order.Amount.ToString() }
    });
```

### 4.4 ABAC 策略评估

需要行级数据权限控制时，调用 Identity gRPC 策略评估接口：

```csharp
// 评估策略（如：只能查看本部门订单、金额超过 10 万需要审批）
var result = await _policyClient.EvaluatePolicyAsync(
    new EvaluatePolicyRequest
    {
        PolicyId = "order-view-scope",
        TenantId = _userContext.TenantId,
        UserId = _userContext.UserId,
        Facts =
        {
            ["user.department"] = _userContext.Organizations.FirstOrDefault(),
            ["user.tags"] = string.Join(",", _userContext.Tags),
            ["resource.owner_id"] = order.CreatedBy,
            ["resource.amount"] = order.Amount.ToString(),
            ["env.current_time"] = DateTime.UtcNow.ToString("O")
        }
    });

if (!result.Allowed)
{
    throw new BusinessException("策略不允许此操作", code: 1003);
}
```

ABAC 典型场景：

| 场景 | 策略示例 | 说明 |
|------|---------|------|
| 数据行级可见 | 只能查看本组织创建的数据 | user.department == resource.department |
| 金额审批 | 超过阈值需要特定角色 | resource.amount > 100000 AND "finance_approver" IN user.roles |
| 时间限制 | 工作时间才能操作 | env.hour >= 9 AND env.hour <= 18 |
| 地域限制 | 只能管理属地数据 | user.region == resource.region |
| 标签匹配 | VIP 客户专属功能 | "vip" IN user.tags |

### 4.5 Service Account（后台任务）

后台任务没有用户身份，使用 Service Account 认证：

```csharp
// 1. 在 Identity 中注册 Service Account（管理员操作）
// POST /api/identity/service-accounts
// { "name": "order-sync-task", "tenantCode": "default" }
// 返回 client_id + client_secret

// 2. 后台任务获取 Token
// POST /api/identity/auth/token
// { "grant_type": "client_credentials", "client_id": "...", "client_secret": "..." }

// 3. 使用 Token 调用业务服务
// 与普通用户 Token 一样通过网关，网关正常解析和签名注入
// 区别：X-User-Type = "service_account"
```

三种后台任务场景：

| 场景 | 身份 | 说明 |
|------|------|------|
| 系统定时任务 | Service Account 自身身份 | X-User-Id = svc:{service-id} |
| 用户触发的异步任务 | Service Account + 原始用户 | X-User-Id = svc-id, X-Original-User-Id = 原用户 |
| MQ 消费者 | Service Account 自身身份 | 消息体中携带触发用户 ID |

---

## 5. 多租户开发

### 5.1 自动租户隔离

TC.Micro 框架通过 `TC.Micro.Identity.Client` 包自动处理多租户隔离：

```csharp
// IUserContext 自动携带租户信息
var tenantId = _userContext.TenantId;

// EF Core 自动按租户过滤（通过 Global Query Filter）
// 业务代码只需关注业务逻辑，框架自动附加 tenant_id 条件
```

### 5.2 数据库设计

每个业务表**必须**包含 `tenant_id` 字段：

```csharp
// Entity 定义
public class Order
{
    public string Id { get; set; }
    public string TenantId { get; set; }  // 必须包含
    public string OrderNo { get; set; }
    // ...
}

// DbContext 配置
modelBuilder.Entity<Order>(entity =>
{
    entity.ToTable("orders");
    entity.Property(e => e.TenantId).IsRequired().HasMaxLength(36);
    entity.HasIndex(e => e.TenantId);

    // Global Query Filter（框架提供基类，自动配置）
    entity.HasQueryFilter(e => e.TenantId == _currentTenantId);
});
```

### 5.3 跨租户禁止

```csharp
// 禁止：直接使用其他租户 ID 查询
var orders = await _db.Orders.Where(o => o.TenantId == "other-tenant").ToListAsync();

// 正确：始终使用 IUserContext.TenantId
var orders = await _db.Orders.Where(o => o.TenantId == _userContext.TenantId).ToListAsync();

// 推荐：使用 Global Query Filter，完全不需要手动过滤
var orders = await _db.Orders.ToListAsync(); // 自动过滤当前租户
```

---

## 6. 服务间通信规范

### 6.1 通信协议策略

```
前端/APP → YARP 网关 → 业务服务 → 业务服务
  HTTP      HTTP↔gRPC      gRPC        gRPC
 (JSON)    (协议转换)   (Protobuf)  (Protobuf)
```

| 链路 | 协议 | 说明 |
|------|------|------|
| 前端 → 网关 | HTTP REST | 浏览器/移动端友好，JSON 可读 |
| 网关 → 业务服务 | gRPC | 网关自动 HTTP ↔ gRPC 协议转换 |
| 服务 → 服务 | gRPC | 内网通信，低延迟高吞吐 |

### 6.2 通信方式选择

| 场景 | 方式 | 说明 |
|------|------|------|
| 同步查询 | gRPC | 高性能，强类型 |
| 同步命令 | HTTP API（经网关） | 需要鉴权审计 |
| 异步通知 | 消息队列（Integration Event） | 解耦、最终一致 |
| 跨服务事务 | DTM TCC | 强一致性 |

### 6.3 Proto 契约使用

所有 gRPC 契约由独立仓库 [TC.Micro.Proto](https://github.com/tangyn75/TC.Micro.Proto) 统一管理（buf CLI + Git + CI/CD），按模块独立发布 NuGet 包，支持 .NET/Go/Python/Java/TypeScript 多语言客户端。

```bash
# .NET 服务安装契约包（按模块引用）
dotnet add package TC.Micro.Proto.Common     # 通用类型（分页、空响应等）
dotnet add package TC.Micro.Proto.Auth       # 统一身份中心（认证+RBAC+ABAC 授权）

# 业务模块按需新增（在 TC.Micro.Proto 仓库添加 proto 文件 → CI 自动发布）
dotnet add package TC.Micro.Proto.Order      # 订单服务（示例）
dotnet add package TC.Micro.Proto.Inventory  # 库存服务（示例）

# 非 .NET 服务从 TC.Micro.Proto 仓库拉取 buf generate 生成的客户端代码
# Go: go get github.com/tangyn75/TC.Micro.Proto/gen/go/...
# Python: pip install tcmicro-proto-auth
# Java: Maven 依赖
# TypeScript: npm 包
# 不支持的语言直接拉取 proto/ 目录自行生成
```

### 6.4 gRPC 调用

```csharp
// 注册 gRPC 客户端（使用 TC.Micro.Proto.Auth 包中的类型）
builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(
    o => o.Address = new Uri("http://identity-service:5201"));

// 使用
public class OrderService
{
    private readonly AuthService.AuthServiceClient _authClient;

    public async Task<bool> CheckPermission(string permissionCode)
    {
        var response = await _authClient.CheckPermissionAsync(new CheckPermissionRequest
        {
            UserId = _userContext.UserId,
            TenantId = _userContext.TenantId,
            PermissionCode = permissionCode
        });
        return response.Allowed;
    }
}
```

### 6.5 消息事件

```csharp
// 定义事件
public class OrderCreatedEvent : IntegrationEvent
{
    public string OrderId { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }  // 事件必须携带租户 ID
    public decimal Amount { get; set; }
}

// 发布
await _publisher.PublishAsync(new OrderCreatedEvent { OrderId = "123", TenantId = _userContext.TenantId, ... });

// 消费
public class OrderCreatedHandler : IMessageConsumer<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        // 处理逻辑，注意使用 message.TenantId 保持租户隔离
    }
}
```

---

## 7. 分布式事务使用规范

### 7.1 何时使用

| 场景 | 方式 |
|------|------|
| 跨服务资金/库存操作 | DTM TCC（强一致） |
| 跨服务状态变更（非关键） | 消息队列（最终一致） |
| 单服务内操作 | 本地事务（无分布式） |

### 7.2 TCC 分支实现规范

```csharp
// Controller 中实现 TCC 分支
[HttpPost("tcc/try/{gid}/{branchId}")]
public async Task<IActionResult> Try(string gid, string branchId, [FromBody] DeductRequest req)
{
    // 1. 幂等检查
    if (await _cache.ExistsAsync($"dtm:processed:{gid}:{branchId}"))
        return Ok("SUCCESS");

    // 2. 防悬挂检查
    if (await _cache.ExistsAsync($"dtm:cancelled:{gid}:{branchId}"))
        return BadRequest("ALREADY_CANCELLED");

    // 3. 业务：冻结库存
    await _inventoryService.FreezeAsync(req.SkuId, req.Qty);

    // 4. 记录幂等
    await _cache.SetAsync($"dtm:processed:{gid}:{branchId}", "1", TimeSpan.FromHours(24));

    return Ok("SUCCESS");
}
```

---

## 8. 数据库规范

### 8.1 数据库选型

| 配置 | 值 |
|------|-----|
| 数据库 | PostgreSQL 16+ |
| ORM | EF Core 8 + Npgsql |
| 扩展 | pgvector（可选，向量搜索） |

### 8.2 数据库隔离

- 每个微服务独立数据库（或独立 Schema）
- 禁止跨库 JOIN
- 禁止跨库事务
- 数据共享通过 API/gRPC/消息

### 8.3 EF Core 规范

```csharp
// DbContext 命名，继承框架提供的基类
public class AppDbContext : DbContext { }

// 使用 Npgsql 配置 PostgreSQL 特有类型
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Order>(entity =>
    {
        entity.ToTable("orders");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasMaxLength(36);
        entity.Property(e => e.TenantId).IsRequired().HasMaxLength(36);
        entity.HasIndex(e => e.TenantId);

        // PostgreSQL JSONB 类型（用于存储灵活的扩展字段）
        entity.Property(e => e.Metadata)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}");
    });
}

// 连接字符串配置
// "Host=localhost;Port=5432;Database=order_db;Username=tc_micro;Password=***"
```

### 8.4 PostgreSQL 注意事项

| 项目 | 说明 |
|------|------|
| 主键 | 使用 UUID 或雪花 ID，避免自增 ID 暴露业务量 |
| JSONB | 适合扩展字段、元数据，支持索引（GIN） |
| 事务隔离 | 默认 Read Committed，需要 Serializable 时显式指定 |
| 批量操作 | 使用 EF Core.ExecuteUpdate/Delete 或 Npgsql 批量拷贝 |
| 连接池 | Npgsql 默认连接池，注意 MaxPoolSize 配置 |

---

## 9. 配置管理规范

### 9.1 配置存储职责

| 配置类型 | 存储位置 | 说明 |
|---------|---------|------|
| 业务服务运行时配置 | Consul KV | 连接串、开关、超时等 |
| 网关管理配置 | PostgreSQL | 路由/限流/熔断/签名密钥，通过管理 UI 操作 |
| 敏感配置 | Consul KV 或 K8s Secret | 密码、密钥等 |

### 9.2 业务服务配置优先级

```
Consul KV (最高) > appsettings.Production.json > appsettings.json (最低)
```

### 9.3 敏感配置

- JWT 密钥、数据库密码、MQ 密码等敏感配置
- **必须**通过 Consul KV 或 K8s Secret 管理
- **禁止**写入 appsettings.json 提交到代码仓库

### 9.3 Consul KV 路径规范

```
TC.Micro/                              # 根前缀
├── {Environment}/                     # 环境（Development/Staging/Production）
│   ├── Database/
│   │   └── ConnectionString           # PostgreSQL 连接串
│   ├── Identity/
│   │   ├── JwtPrivateKey              # RS256 私钥（仅 Identity 使用）
│   │   └── JwtPublicKey               # RS256 公钥（网关和业务服务使用）
│   ├── Gateway/
│   │   └── SigningSecret              # 网关 HMAC 签名密钥
│   └── RabbitMQ/
│       └── Password
```

---

## 10. 日志规范

### 10.1 日志级别使用

| 级别 | 使用场景 |
|------|---------|
| Error | 异常、外部调用失败 |
| Warning | 业务异常、重试中 |
| Information | 关键业务流程（订单创建、支付完成） |
| Debug | 详细调试信息 |
| Trace | 极详细（仅开发环境） |

### 10.2 结构化日志

```csharp
// 正确
_logger.LogInformation("Order {OrderId} created by user {UserId} in tenant {TenantId}, amount: {Amount}",
    order.Id, _userContext.UserId, _userContext.TenantId, order.Amount);

// 错误 — 不要使用字符串插值
_logger.LogInformation($"Order {order.Id} created by user {userId}");
```

---

## 11. 测试规范

| 类型 | 覆盖范围 | 工具 |
|------|---------|------|
| 单元测试 | Service 层逻辑 | xUnit + Moq |
| 集成测试 | API 端到端 | WebApplicationFactory |
| 契约测试 | gRPC proto 兼容性 | 手动验证 |

---

## 12. Git 规范

### 12.1 分支策略

```
main            ← 生产分支
├── develop     ← 开发分支
├── feature/*   ← 功能分支
├── fix/*       ← 修复分支
└── release/*   ← 发布分支
```

### 12.2 提交信息

```
feat: add order creation endpoint
fix: handle inventory deduction race condition
refactor: extract common validation logic
test: add unit tests for order service
docs: update API documentation
chore: upgrade Serilog to 4.x
```

---

## 13. 关键依赖包版本

| 包名 | 版本 | 用途 |
|------|------|------|
| Yarp.ReverseProxy | 2.x | 内部网关 |
| Consul | 1.x | 服务注册/发现/配置 |
| Dtmcli | 1.x | 分布式事务 |
| RabbitMQ.Client | 6.x/7.x | RabbitMQ |
| Confluent.Kafka | 2.x | Kafka |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.x | PostgreSQL EF Core 提供程序 |
| Serilog.* | 4.x | 结构化日志 |
| OpenTelemetry.* | 1.x | 链路追踪 |
| StackExchange.Redis | 2.x | Redis |
| Microsoft.AspNetCore.OpenApi | 9.x | .NET 原生 OpenAPI spec 生成 |
| Scalar.AspNetCore | 1.x | 统一 API 文档（网关聚合） |
| Grpc.AspNetCore | 2.x | gRPC |
| Polly | 8.x | 熔断/重试 |
