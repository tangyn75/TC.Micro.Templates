# TC.Micro 业务开发规范

> 版本：2.0 | 日期：2026-04-19
> 变更：v2.0 优化结构，突出业务开发重点，补充部署、监控、常见问题等实用内容

---

## 🚀 快速入门

### 1.1 新建业务服务

使用 `tcmicro-webapi` 模板一键生成项目：

```bash
# 基础 HTTP 服务（三层结构）
dotnet new tcmicro-webapi -n TC.Micro.OrderService

# 包含内置管理后台（可选 Server/WASM/Auto 模式）
dotnet new tcmicro-webapi -n TC.Micro.OrderService --IncludeAdminUi --AdminHostingMode server

# 混合模式（HTTP + gRPC）
dotnet new tcmicro-webapi -n TC.Micro.OrderService --ApiType httpAndGrpc
```

生成的项目结构：
```
TC.Micro.OrderService/
├── TC.Micro.OrderService.Api/          # API 层（HTTP/gRPC 入口）
├── TC.Micro.OrderService.Application/  # 业务层（业务逻辑、DTO、事件）
├── TC.Micro.OrderService.Infrastructure/ # 数据层（EF Core、实体、外部客户端）
├── [TC.Micro.OrderService.Admin/]      # 可选：内置管理后台（--IncludeAdminUi 时生成）
├── [TC.Micro.OrderService.Admin.Client/] # 可选：Auto 模式 WASM 客户端
├── TC.Micro.OrderService.sln
└── docs/                               # 开发规范文档
```

### 1.2 开发流程

1. **修改配置**：更新 `Api/appsettings.json` 中的服务名称、端口等配置
2. **定义实体**：在 `Infrastructure/Entities/` 添加数据库实体
3. **编写业务**：在 `Application/Services/` 实现业务逻辑
4. **暴露接口**：在 `Api/Controllers/` 编写 API 接口
5. **本地测试**：运行服务，通过网关统一文档 `http://gateway:5100/scalar` 调试
6. **提交代码**：遵循 Git 提交规范推送到仓库

### 1.3 上线前检查清单

- ✅ 数据库表已创建，包含 `tenant_id` 字段
- ✅ 服务已在 Identity 注册，权限码配置完成
- ✅ 网关路由已配置，权限校验规则正确
- ✅ 健康检查端点正常：`/health`
- ✅ 配置已同步到 Consul KV，无硬编码敏感信息
- ✅ 单元测试覆盖率 ≥ 80%

---

## 📋 核心开发规范

### 2.1 项目结构规范

#### 三层结构职责划分

| 层级 | 项目 | 职责 | 禁止 |
|------|------|------|------|
| API层 | `*.Api` | 接口入口、参数校验、路由配置 | 编写业务逻辑、直接操作数据库 |
| 业务层 | `*.Application` | 业务逻辑实现、DTO定义、事件处理 | 直接返回 HTTP 状态码、引入 ASP.NET Core 依赖 |
| 数据层 | `*.Infrastructure` | 数据库操作、外部服务客户端 | 编写业务逻辑 |

**项目引用关系**：`Api → Application → Infrastructure`

### 2.2 命名规范

#### C# 代码命名
| 类型 | 规范 | 示例 |
|------|------|------|
| Controller | {资源名}Controller | OrderController |
| Service 接口 | I{业务名}Service | IOrderService |
| Service 实现 | {业务名}Service | OrderService |
| 实体 | {业务名}（无后缀） | Order |
| 请求 DTO | {操作}{资源}Request | CreateOrderRequest |
| 响应 DTO | {资源}Response | OrderResponse |
| 集成事件 | {操作}{资源}Event | OrderCreatedEvent |
| 事件处理器 | {事件名}Handler | OrderCreatedHandler |

#### API 接口命名（统一小写蛇形）
| 范围 | 规范 | 示例 |
|------|------|------|
| JSON 字段 | snake_case | `user_id`, `created_at` |
| URL 路径 | kebab-case | `/api/orders/{order_id}` |
| Query 参数 | snake_case | `?page_index=1&page_size=20` |
| Header | 首字母大写 + 连字符 | `X-User-Id`, `X-Tenant-Id` |

> ✅ 最佳实践：DTO 属性使用 PascalCase，框架自动序列化转换为 snake_case，**不要手动加 `[JsonPropertyName]`**

### 2.3 API 响应规范

所有接口统一返回 `ServiceResult<T>` 格式：
```json
{
  "code": 0,          // 0=成功，非0=错误码
  "message": "success", // 错误信息
  "data": {},         // 返回数据
  "timestamp": 1713244800000
}
```

**通用错误码**：
| 错误码 | 含义 |
|--------|------|
| 0 | 成功 |
| -1 | 通用业务错误 |
| 1001 | 参数校验失败 |
| 1002 | 资源不存在 |
| 1003 | 权限不足 |
| 2001 | 数据库错误 |
| 2002 | 外部服务调用失败 |

### 2.4 异常处理规范

```csharp
// 业务异常直接抛出，全局中间件自动处理
throw new BusinessException("订单不存在", code: 1002);

// 参数校验失败使用 FluentValidation
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("订单金额必须大于0");
    }
}
```

---

## 🔧 业务开发指南

### 3.1 身份与鉴权

#### 获取当前用户信息
```csharp
// 直接注入 IUserContext 使用
public class OrderService
{
    private readonly IUserContext _userContext;
    
    public async Task CreateOrder(CreateOrderRequest request)
    {
        var userId = _userContext.UserId;        // 当前用户ID
        var tenantId = _userContext.TenantId;    // 当前租户ID
        var roles = _userContext.Roles;          // 用户角色列表
        var permissions = _userContext.Permissions; // 用户权限列表
    }
}
```

#### 权限校验
```csharp
// 1. 路由级权限：网关自动校验，业务无需处理
// 2. 细粒度权限校验
if (!_userContext.HasPermission("order:approve"))
{
    throw new BusinessException("无订单审批权限", code: 1003);
}

// 3. ABAC 行级权限（如：只能审批金额<10万的订单）
var allowed = await _authService.CheckPolicyAsync("order-approve-policy", new 
{
    Amount = request.Amount,
    UserId = _userContext.UserId
});
```

### 3.2 多租户开发

框架自动实现租户隔离，业务开发只需遵循：
1. ✅ 所有业务表必须包含 `tenant_id` 字段
2. ✅ 使用 `_userContext.TenantId` 获取当前租户
3. ✅ 依赖框架的 Global Query Filter 自动过滤租户数据
4. ❌ 禁止硬编码租户 ID 查询其他租户数据

```csharp
// 正确：无需手动过滤，框架自动处理
var orders = await _db.Orders.ToListAsync();

// 禁止：手动指定租户ID
var orders = await _db.Orders.Where(o => o.TenantId == "xxx").ToListAsync();
```

### 3.3 数据库开发规范

#### 实体定义要求
```csharp
public class Order : IEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } // 必填，框架自动填充
    public string OrderNo { get; set; }
    public decimal Amount { get; set; }
    public string CreatedBy { get; set; } // 记录创建人
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // 软删除标记
}
```

#### EF Core 使用规范
- ✅ 禁用 Repository 层，Service 直接注入 `AppDbContext` 使用
- ✅ 批量操作使用 `ExecuteUpdate/Delete`，避免循环查询
- ✅ JSON 扩展字段使用 PostgreSQL `jsonb` 类型
- ❌ 禁止使用原生 SQL 拼接，避免注入风险

### 3.4 服务间通信

#### 同步调用（gRPC）
```csharp
// 1. 安装对应 Proto 包
dotnet add package TC.Micro.Proto.Inventory

// 2. 注册客户端
builder.Services.AddGrpcClient<InventoryService.InventoryServiceClient>(
    o => o.Address = new Uri("http://inventory-service:5201"));

// 3. 调用
public class OrderService
{
    private readonly InventoryService.InventoryServiceClient _inventoryClient;
    
    public async Task CreateOrder(CreateOrderRequest request)
    {
        await _inventoryClient.DeductAsync(new DeductRequest
        {
            SkuId = request.SkuId,
            Qty = request.Qty
        });
    }
}
```

#### 异步通信（消息队列）
```csharp
// 1. 定义事件
public class OrderCreatedEvent : IntegrationEvent
{
    public string OrderId { get; set; }
    public string TenantId { get; set; } // 必须携带租户ID
    public decimal Amount { get; set; }
}

// 2. 发布事件
await _eventPublisher.PublishAsync(new OrderCreatedEvent
{
    OrderId = order.Id,
    TenantId = _userContext.TenantId,
    Amount = order.Amount
});

// 3. 消费事件
public class OrderCreatedHandler : IMessageConsumer<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        // 处理逻辑，使用 message.TenantId 保持租户隔离
    }
}
```

### 3.5 分布式事务

**使用场景**：
- 跨服务资金/库存扣减等强一致性场景 → DTM TCC
- 非关键路径状态变更 → 消息队列最终一致

```csharp
// TCC Try 方法实现示例
[HttpPost("tcc/try/{gid}/{branchId}")]
public async Task<IActionResult> TryDeduct(string gid, string branchId, DeductRequest req)
{
    // 1. 幂等检查
    if (await _cache.ExistsAsync($"dtm:processed:{gid}:{branchId}"))
        return Ok("SUCCESS");
        
    // 2. 业务逻辑：冻结库存
    await _inventoryService.FreezeAsync(req.SkuId, req.Qty);
    
    // 3. 记录处理标记
    await _cache.SetAsync($"dtm:processed:{gid}:{branchId}", "1", TimeSpan.FromHours(24));
    
    return Ok("SUCCESS");
}
```

---

## 🚢 运维与部署

### 4.1 配置管理

**配置优先级**：`Consul KV > 环境变量 > appsettings.Production.json > appsettings.json`

**敏感配置要求**：
- ❌ 禁止将数据库密码、密钥等敏感信息提交到代码仓库
- ✅ 敏感配置必须通过 Consul KV 或 K8s Secret 管理
- ✅ 本地开发使用 `appsettings.Development.json`（已加入 .gitignore）

### 4.2 日志规范

```csharp
// ✅ 正确：使用结构化日志，不要字符串插值
_logger.LogInformation("Order {OrderId} created by user {UserId}, amount: {Amount}",
    order.Id, _userContext.UserId, order.Amount);

// ❌ 错误
_logger.LogInformation($"Order {order.Id} created by {userId}");
```

**日志级别使用**：
| 级别 | 场景 |
|------|------|
| Error | 异常、外部调用失败 |
| Warning | 业务异常、重试、非关键错误 |
| Information | 关键业务节点（订单创建、支付完成） |
| Debug | 调试信息，生产环境默认关闭 |

### 4.3 健康检查

模板已自动配置健康检查端点：
- 健康检查：`/health`
- 就绪检查：`/health/ready`
- 存活检查：`/health/live`

**扩展自定义检查**：
```csharp
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom_check");
```

### 4.4 监控与告警

核心指标自动采集：
- QPS、请求耗时、错误率
- 数据库连接池使用情况
- GC、内存、CPU 使用率

**业务自定义指标**：
```csharp
// 记录订单创建数量
_meter.Counter("order_created_count").Add(1, 
    new KeyValuePair<string, object>("tenant_id", _userContext.TenantId));
```

---

## 📝 协作规范

### 5.1 Git 提交规范

提交信息格式：`type: description`
```
feat: add order creation endpoint       # 新增功能
fix: resolve inventory race condition   # 修复bug
refactor: simplify order service logic  # 代码重构
test: add unit tests for order service  # 测试相关
docs: update API documentation          # 文档更新
chore: upgrade Serilog to 4.x           # 构建/依赖调整
```

### 5.2 分支策略
```
main            # 生产分支，tag 标记版本
├── develop     # 开发分支
├── feature/*   # 功能分支，从 develop 创建
├── fix/*       # Bugfix分支
└── release/*   # 发布分支
```

---

## ❓ 常见问题

### Q：如何新增 API 接口？
A：在 `Api/Controllers/` 新增 Controller，继承 `ControllerBase`，添加对应接口方法，DTO 放在 `Application/DTOs/`。

### Q：如何添加 gRPC 服务？
A：创建 `.proto` 文件，配置 `Grpc.AspNetCore`，服务实现放在 `Api/Services/`。

### Q：如何处理文件上传？
A：Controller 中使用 `IFormFile` 接收，文件存储到对象存储（OSS/MinIO），不要存在本地。

### Q：如何实现定时任务？
A：使用 `Hangfire` 或独立的调度服务，不要在业务服务中写定时器。

---

## 📚 附录

### 关键依赖版本
| 包名 | 版本 | 用途 |
|------|------|------|
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.x | PostgreSQL EF Core |
| Serilog.AspNetCore | 4.x | 结构化日志 |
| StackExchange.Redis | 2.x | Redis 客户端 |
| Grpc.AspNetCore | 2.x | gRPC 服务/客户端 |
| Polly | 8.x | 熔断重试 |
| FluentValidation.AspNetCore | 11.x | 参数校验 |