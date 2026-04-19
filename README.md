# TC.Micro Templates

dotnet new templates for TC.Micro microservice framework.

## Templates

| Short Name | Description |
|------------|-------------|
| `tcmicro-webapi` | Business WebAPI service (Api / Application / Infrastructure [/ Admin]), supports HTTP, gRPC, or mixed mode |
| `tcmicro-blazoradmin` | Standalone Blazor Server admin portal (MudBlazor, dark/light theme, i18n, JWT auth) |

## Install

```bash
dotnet new install TC.Micro.Templates
```

## Usage

### tcmicro-webapi

```bash
# HTTP service (default)
dotnet new tcmicro-webapi -n TC.Micro.OrderService

# gRPC service
dotnet new tcmicro-webapi -n TC.Micro.Inventory --ApiType grpc

# HTTP + gRPC mixed mode with built-in admin UI
dotnet new tcmicro-webapi -n TC.Micro.Payment --ApiType httpAndGrpc --IncludeAdminUi

# Custom ports
dotnet new tcmicro-webapi -n TC.Micro.OrderService --ServicePort 5410 --AdminPort 5411
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `-n` | string | (required) | Service name |
| `--ServicePort` | int | 5400 | HTTP/gRPC port (5400-5599 range) |
| `--AdminPort` | int | 5401 | Admin port (only with `--IncludeAdminUi`) |
| `--ApiType` | choice | http | `http` / `grpc` / `httpAndGrpc` |
| `--IncludeAdminUi` | bool | false | Generate Admin project (4th project) |
| `--Framework` | choice | net8.0 | `net8.0` / `net9.0` |

### tcmicro-blazoradmin

```bash
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin

# Custom ports and API URLs
dotnet new tcmicro-blazoradmin -n TC.Micro.MyAdmin \
  --AdminPort 6200 \
  --GatewayManagementUrl http://gateway:5101/api \
  --IdentityManagementUrl http://identity:5202/api
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `-n` | string | (required) | Project name |
| `--AdminPort` | int | 6100 | HTTP port |
| `--GatewayManagementUrl` | string | http://localhost:5101/api | Gateway admin API URL |
| `--IdentityManagementUrl` | string | http://localhost:5202/api | Identity admin API URL |
| `--Framework` | choice | net8.0 | `net8.0` / `net9.0` |

## Development Standards

Each template includes embedded development standards:

- **tcmicro-webapi**: `docs/development-guide.md` — backend coding conventions, API design, authentication, multi-tenancy, database, messaging
- **tcmicro-blazoradmin**: `docs/MudBlazor-Admin-Design-Standard.md` — UI design system, component specs, responsive layout, i18n, theming

## Build NuGet Package

```bash
dotnet pack TC.Micro.Templates.csproj
```

## Uninstall

```bash
dotnet new uninstall TC.Micro.Templates
```
