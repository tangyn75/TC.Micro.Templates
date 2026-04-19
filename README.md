# TC.Micro 项目模板

TC.Micro 微服务框架的 dotnet new 模板集合，包含业务服务和管理后台模板，内置统一开发规范。

## 模板列表

| 短名称 | 描述 |
|--------|------|
| `tcmicro-webapi` | 业务微服务模板（三层结构：Api / Application / Infrastructure），支持 HTTP、gRPC 或混合模式，可选内置管理后台 |
| `tcmicro-blazoradmin` | 独立管理后台模板（Blazor 全托管模式支持：Server / WASM / Auto），基于 MudBlazor 8.x，内置深色/浅色主题、多语言、JWT 认证 |

## 安装

```bash
dotnet new install TC.Micro.Templates
```

## 使用说明

### tcmicro-webapi 业务服务模板

```bash
# 默认 HTTP 服务（三层结构）
dotnet new tcmicro-webapi -n TC.Micro.OrderService

# 纯 gRPC 内部服务
dotnet new tcmicro-webapi -n TC.Micro.Inventory --ApiType grpc

# HTTP + gRPC 混合模式，包含内置管理后台（Server 模式，默认）
dotnet new tcmicro-webapi -n TC.Micro.Payment --ApiType httpAndGrpc --IncludeAdminUi

# 内置管理后台使用 WASM 模式
dotnet new tcmicro-webapi -n TC.Micro.Payment --IncludeAdminUi --AdminHostingMode wasm

# 内置管理后台使用 Auto 模式（SSR + WASM 交互，双项目）
dotnet new tcmicro-webapi -n TC.Micro.Payment --IncludeAdminUi --AdminHostingMode auto

# 自定义端口
dotnet new tcmicro-webapi -n TC.Micro.OrderService --ServicePort 5410 --AdminPort 5411
```

#### 参数说明

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `-n` | string | 必填 | 服务名称 |
| `--ServicePort` | int | 5400 | 业务服务 HTTP/gRPC 端口（范围 5400-5599） |
| `--AdminPort` | int | 5401 | 管理后台端口（仅 `--IncludeAdminUi` 时生效） |
| `--ApiType` | choice | http | 服务类型：`http` / `grpc` / `httpAndGrpc` |
| `--IncludeAdminUi` | bool | false | 是否生成内置管理后台项目（第四层） |
| `--AdminHostingModel` | choice | server | 管理后台托管模式：`server` / `wasm` / `auto`（仅 `--IncludeAdminUi` 时生效） |
| `--Framework` | choice | net8.0 | 目标框架：`net8.0` / `net9.0` |

### tcmicro-blazoradmin 独立管理后台模板

```bash
# 生成独立管理后台（默认 Server 模式）
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin

# 自定义端口和 API 地址
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin \
  --AdminPort 6200 \
  --GatewayManagementUrl http://gateway:5101/api \
  --IdentityManagementUrl http://identity:5202/api

# 使用 WASM 模式
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin --HostingModel wasm

# 使用 Auto 模式
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin --HostingModel auto
```

#### 参数说明

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `-n` | string | 必填 | 项目名称 |
| `--AdminPort` | int | 6100 | HTTP 服务端口 |
| `--HostingModel` | choice | server | Blazor 托管模式：`server` / `wasm` / `auto` |
| `--GatewayManagementUrl` | string | http://localhost:5101/api | 网关管理 API 地址 |
| `--IdentityManagementUrl` | string | http://localhost:5202/api | 身份中心管理 API 地址 |
| `--Framework` | choice | net8.0 | 目标框架：`net8.0` / `net9.0` |

## 内置开发规范

每个模板生成时会自动包含对应开发规范文档：

- **tcmicro-webapi**: `docs/development-guide.md` — 后端编码规范、API 设计、认证鉴权、多租户、数据库、服务间通信等规范
- **tcmicro-blazoradmin**: `docs/MudBlazor-Admin-Design-Standard.md` — UI 设计系统、组件规范、响应式布局、多语言、主题定制等规范

## 构建 NuGet 包

```bash
dotnet pack TC.Micro.Templates.csproj -c Release
```

## 卸载

```bash
dotnet new uninstall TC.Micro.Templates
```
