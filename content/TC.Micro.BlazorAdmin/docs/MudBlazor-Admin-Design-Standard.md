# MudBlazor 企业级管理后台设计标准文档

> **版本**: v1.0.0  
> **基于**: MudBlazor 8.x · Material Design 3 · .NET 8 Blazor  
> **模式**: SPA（单页应用）· 非多 Tab  
> **参考风格**: Ant Design Pro · Material Design 规范  

---

## 目录

1. [项目概述](#1-项目概述)
2. [技术架构](#2-技术架构)
3. [设计系统](#3-设计系统)
4. [布局规范](#4-布局规范)
5. [响应式设计](#5-响应式设计)
6. [主题系统（明/暗）](#6-主题系统)
7. [多语言国际化](#7-多语言国际化)
8. [路由与导航](#8-路由与导航)
9. [组件规范](#9-组件规范)
10. [页面模板](#10-页面模板)
11. [Blazor 双模式支持](#11-blazor-双模式支持)
12. [项目结构](#12-项目结构)
13. [开发规范](#13-开发规范)
14. [性能优化](#14-性能优化)
15. [部署指南](#15-部署指南)

---

## 1. 项目概述

### 1.1 设计目标

| 目标 | 说明 |
|------|------|
| **专业性** | 对标 Ant Design Pro、Material Admin 等企业级产品 |
| **一致性** | 全局统一设计语言，基于 Material Design 3 规范 |
| **可维护性** | 模块化组件，低耦合高内聚 |
| **可扩展性** | 支持业务快速扩展，主题可配置 |
| **可访问性** | WCAG 2.1 AA 级别，支持键盘导航 |

### 1.2 核心特性

```
✅ SPA 模式（非多 Tab，类 SAP Fiori 风格）
✅ 响应式布局（Mobile / Tablet / Desktop）
✅ 明暗主题切换（含系统跟随）
✅ 多语言支持（i18n，运行时切换）
✅ Blazor Server & WebAssembly 双模式
✅ 基于 MudBlazor 8.x + Material Design 3
✅ JWT 认证 + 角色权限控制
✅ 动态菜单（后端驱动）
```

### 1.3 浏览器兼容性

| 浏览器 | 最低版本 |
|--------|----------|
| Chrome | 90+ |
| Firefox | 88+ |
| Edge | 90+ |
| Safari | 14+ |
| 移动端 | iOS 14+ / Android 10+ |

---

## 2. 技术架构

### 2.1 技术栈

```
框架层:     .NET 8 + Blazor (Server / WebAssembly)
UI 库:      MudBlazor 8.x
CSS:         MudBlazor 主题系统 + CSS 变量
状态管理:   Fluxor 或 CascadingValue / StateContainer
国际化:     Microsoft.Extensions.Localization
HTTP:        HttpClient + Refit / RestSharp
认证:        ASP.NET Core Identity + JWT Bearer
日志:        Serilog + Seq
```

### 2.2 架构分层

```
┌─────────────────────────────────────────────────┐
│                  Presentation Layer              │
│   Pages / Components / Layouts / Shared         │
├─────────────────────────────────────────────────┤
│                  Application Layer               │
│   ViewModels / Services / State / Validators    │
├─────────────────────────────────────────────────┤
│                  Infrastructure Layer            │
│   HttpClients / Repositories / Auth / I18n      │
├─────────────────────────────────────────────────┤
│                  Domain Layer                    │
│   Models / Enums / Constants / Interfaces       │
└─────────────────────────────────────────────────┘
```

### 2.3 依赖包清单

```xml
<!-- 核心 UI -->
<PackageReference Include="MudBlazor" Version="8.*" />

<!-- 图标 -->
<PackageReference Include="MudBlazor.Extensions" Version="8.*" />

<!-- 多语言 -->
<PackageReference Include="Microsoft.Extensions.Localization" Version="8.*" />

<!-- 状态管理 -->
<PackageReference Include="Fluxor.Blazor.Web" Version="6.*" />

<!-- HTTP 客户端 -->
<PackageReference Include="Refit" Version="7.*" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.*" />

<!-- 验证 -->
<PackageReference Include="FluentValidation" Version="11.*" />

<!-- 图表（可选） -->
<PackageReference Include="Blazor-ApexCharts" Version="*" />

<!-- 认证 -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.*" />

<!-- 日志 -->
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
```

---

## 3. 设计系统

### 3.1 色彩系统

遵循 Material Design 3 Color System，支持明暗两套完整色板。

#### 3.1.1 主色板（Brand Palette）

```scss
// ── 主色（Primary）──
--mud-palette-primary:          #1976D2;   // Blue 700（亮色模式）
--mud-palette-primary-darken:   #1565C0;
--mud-palette-primary-lighten:  #42A5F5;
--mud-palette-primary-text:     #FFFFFF;

// ── 次要色（Secondary）──
--mud-palette-secondary:        #00BCD4;   // Cyan 500
--mud-palette-secondary-darken: #0097A7;
--mud-palette-secondary-text:   #FFFFFF;

// ── 强调色（Tertiary / Accent）──
--mud-palette-tertiary:         #FF6F00;   // Amber 800
--mud-palette-info:             #2196F3;
--mud-palette-success:          #4CAF50;
--mud-palette-warning:          #FF9800;
--mud-palette-error:            #F44336;
```

#### 3.1.2 中性色板

```scss
// ── 表面色 ──
--mud-palette-surface:          #FFFFFF;
--mud-palette-background:       #F5F7FA;   // 页面背景（亮色）
--mud-palette-background-grey:  #F0F2F5;

// ── 边框色 ──
--mud-palette-divider:          rgba(0,0,0,0.12);
--mud-palette-divider-light:    rgba(0,0,0,0.06);

// ── 文字色 ──
--mud-palette-text-primary:     rgba(0,0,0,0.87);
--mud-palette-text-secondary:   rgba(0,0,0,0.60);
--mud-palette-text-disabled:    rgba(0,0,0,0.38);

// ── 暗色模式覆盖 ──
[data-mud-theme="dark"] {
  --mud-palette-background:     #111827;
  --mud-palette-surface:        #1F2937;
  --mud-palette-text-primary:   rgba(255,255,255,0.87);
  --mud-palette-divider:        rgba(255,255,255,0.12);
}
```

#### 3.1.3 功能色语义

| 场景 | 颜色 Token | 用途 |
|------|-----------|------|
| 操作成功 | `--mud-palette-success` | 保存成功、审核通过 |
| 警告提示 | `--mud-palette-warning` | 即将过期、注意事项 |
| 错误/危险 | `--mud-palette-error` | 删除、失败、异常 |
| 信息提示 | `--mud-palette-info` | 一般性说明 |
| 禁用状态 | `--mud-palette-text-disabled` | 不可操作元素 |

### 3.2 字体系统

```scss
// ── 字体族 ──
--mud-typography-default-family:    'Roboto', 'Noto Sans SC', sans-serif;
// 中文回退：Noto Sans SC 覆盖汉字
// 英文优先：Roboto

// ── 字阶定义（T-shirt Scale）──
--font-size-xs:   0.625rem;   // 10px
--font-size-sm:   0.75rem;    // 12px
--font-size-md:   0.875rem;   // 14px  ← 正文基准
--font-size-lg:   1rem;       // 16px
--font-size-xl:   1.25rem;    // 20px
--font-size-2xl:  1.5rem;     // 24px
--font-size-3xl:  2rem;       // 32px

// ── 行高 ──
--line-height-tight:   1.25;
--line-height-normal:  1.5;
--line-height-relaxed: 1.75;

// ── 字重 ──
--font-weight-regular:  400;
--font-weight-medium:   500;
--font-weight-semibold: 600;
--font-weight-bold:     700;
```

#### Material Typography 映射

| MudBlazor Typo | 大小 | 字重 | 用途 |
|----------------|------|------|------|
| `Typo.h4` | 34px | 400 | 页面主标题 |
| `Typo.h5` | 24px | 400 | 卡片标题 |
| `Typo.h6` | 20px | 500 | 区块标题 |
| `Typo.subtitle1` | 16px | 400 | 副标题 |
| `Typo.subtitle2` | 14px | 500 | 表格列头 |
| `Typo.body1` | 16px | 400 | 正文 |
| `Typo.body2` | 14px | 400 | 辅助文字 |
| `Typo.caption` | 12px | 400 | 说明、标签 |
| `Typo.overline` | 12px | 400 | 上方标注 |

### 3.3 间距系统

采用 **4px 基准网格**（Material Design 规范）：

```scss
// ── 间距变量 ──
--spacing-1:  4px;
--spacing-2:  8px;
--spacing-3:  12px;
--spacing-4:  16px;   // 标准内边距
--spacing-5:  20px;
--spacing-6:  24px;   // 卡片内边距
--spacing-8:  32px;
--spacing-10: 40px;
--spacing-12: 48px;
--spacing-16: 64px;

// MudBlazor 使用: Margin="x" / Padding="x"
// x=1 → 4px, x=2 → 8px ... x=6 → 24px
```

### 3.4 圆角系统

```scss
--radius-xs:  2px;
--radius-sm:  4px;
--radius-md:  8px;    // 卡片、对话框
--radius-lg:  12px;   // 大卡片
--radius-xl:  16px;
--radius-full: 9999px; // 标签、按钮 Pill
```

### 3.5 阴影系统

遵循 Material Design Elevation：

```scss
// Elevation 0 → 无阴影（Flat）
// Elevation 1 → 卡片默认
// Elevation 4 → AppBar / Drawer
// Elevation 8 → 下拉菜单
// Elevation 16 → 对话框
// Elevation 24 → 浮动按钮

// MudBlazor 组件: Elevation="1" 等
```

### 3.6 图标规范

统一使用 **Material Icons**（MudBlazor 内置）：

```razor
<!-- 标准用法 -->
<MudIcon Icon="@Icons.Material.Filled.Dashboard" Size="Size.Medium" />
<MudIcon Icon="@Icons.Material.Outlined.Settings" Size="Size.Small" />

<!-- 尺寸规范 -->
Size.Small    → 20px（列表、密集区域）
Size.Medium   → 24px（标准，默认）
Size.Large    → 36px（空状态、强调）
```

**图标风格统一原则**：
- 导航菜单：`Outlined` 风格（未选中）→ `Filled`（选中）
- 操作按钮：`Rounded` 风格
- 状态图标：`Filled` 风格

---

## 4. 布局规范

### 4.1 整体布局结构（SPA 模式）

```
┌────────────────────────────────────────────────────────────┐
│  AppBar（顶部导航栏）                              H: 64px  │
│  [☰ Logo]  [面包屑 / 页面标题]  [搜索][通知][头像]         │
├──────────────┬─────────────────────────────────────────────┤
│              │                                             │
│  NavDrawer   │   Main Content Area                        │
│  (侧边导航)  │                                             │
│  W: 256px    │   ┌─────────────────────────────────────┐  │
│              │   │  Breadcrumb                         │  │
│  [菜单项 1]  │   ├─────────────────────────────────────┤  │
│  [菜单项 2]  │   │                                     │  │
│    [子项]    │   │   Page Content (Router Outlet)      │  │
│    [子项]    │   │                                     │  │
│  [菜单项 3]  │   │                                     │  │
│              │   └─────────────────────────────────────┘  │
│              │                                             │
├──────────────┴─────────────────────────────────────────────┤
│  Footer（可选）                                    H: 48px  │
└────────────────────────────────────────────────────────────┘
```

### 4.2 MainLayout 实现

```razor
<!-- Layouts/MainLayout.razor -->
@inherits LayoutComponentBase
@inject IThemeService ThemeService
@inject ILayoutService LayoutService

<MudThemeProvider Theme="@ThemeService.CurrentTheme" 
                  IsDarkMode="@ThemeService.IsDarkMode" />
<MudDialogProvider FullWidth="true" MaxWidth="MaxWidth.Small" />
<MudSnackbarProvider />
<MudPopoverProvider />

<MudLayout>
    <!-- 顶部 AppBar -->
    <AppBar OnMenuToggle="@ToggleDrawer" />
    
    <!-- 侧边导航抽屉 -->
    <NavDrawer IsOpen="@_drawerOpen" 
               Variant="@_drawerVariant"
               OnClose="@CloseDrawer" />
    
    <!-- 主内容区域 -->
    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.False" Class="pa-4 pa-md-6">
            <!-- 面包屑 -->
            <BreadcrumbNav Class="mb-4" />
            
            <!-- 页面路由出口 -->
            <ErrorBoundary>
                <ChildContent>
                    @Body
                </ChildContent>
                <ErrorContent Context="ex">
                    <ErrorPage Exception="@ex" />
                </ErrorContent>
            </ErrorBoundary>
        </MudContainer>
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;
    private DrawerVariant _drawerVariant = DrawerVariant.Responsive;
    
    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;
    private void CloseDrawer() => _drawerOpen = false;
}
```

### 4.3 AppBar 规范

```razor
<!-- Shared/AppBar.razor -->
<MudAppBar Elevation="1" Dense="false" Color="Color.Surface">
    
    <!-- 左侧：Logo + 菜单切换 -->
    <MudIconButton Icon="@Icons.Material.Filled.Menu" 
                   Color="Color.Inherit" 
                   Edge="Edge.Start"
                   OnClick="@OnMenuToggle" />
    
    <MudHidden Breakpoint="Breakpoint.SmAndDown">
        <MudImage Src="/images/logo.svg" Height="32" Class="ml-2 mr-4" />
        <MudText Typo="Typo.h6" Class="app-title">Admin Pro</MudText>
    </MudHidden>
    
    <!-- 中间：全局搜索（桌面端显示） -->
    <MudHidden Breakpoint="Breakpoint.SmAndDown">
        <GlobalSearch Class="mx-4" Style="min-width: 320px;" />
    </MudHidden>
    
    <MudSpacer />
    
    <!-- 右侧：工具栏 -->
    <MudHidden Breakpoint="Breakpoint.SmAndDown" Invert="true">
        <!-- 移动端搜索图标 -->
        <MudIconButton Icon="@Icons.Material.Filled.Search" Color="Color.Inherit" />
    </MudHidden>
    
    <!-- 语言切换 -->
    <LanguageSwitcher />
    
    <!-- 主题切换 -->
    <ThemeToggle />
    
    <!-- 通知 -->
    <NotificationBell UnreadCount="@_unreadCount" />
    
    <!-- 全屏 -->
    <MudHidden Breakpoint="Breakpoint.SmAndDown">
        <FullscreenToggle />
    </MudHidden>
    
    <!-- 用户头像 -->
    <UserAvatar />
    
</MudAppBar>
```

**AppBar 高度规范**：

| 断点 | Dense | 高度 |
|------|-------|------|
| Desktop | false | 64px |
| Tablet | false | 64px |
| Mobile | true | 56px |

### 4.4 NavDrawer 规范

```razor
<!-- Shared/NavDrawer.razor -->
<MudDrawer @bind-Open="IsOpen"
           Variant="@Variant"
           ClipMode="DrawerClipMode.Always"
           Elevation="2"
           Width="256px">
    
    <!-- 抽屉头部（移动端显示 Logo） -->
    <MudDrawerHeader>
        <MudStack Row="true" AlignItems="AlignItems.Center">
            <MudImage Src="/images/logo-icon.svg" Height="28" />
            <MudText Typo="Typo.h6" Class="ml-2">Admin Pro</MudText>
        </MudStack>
    </MudDrawerHeader>
    
    <MudDivider />
    
    <!-- 导航菜单 -->
    <MudNavMenu Dense="false" Rounded="true" Class="pa-2 flex-1">
        @foreach (var group in _menuGroups)
        {
            <MenuGroup Group="@group" />
        }
    </MudNavMenu>
    
    <MudDivider />
    
    <!-- 抽屉底部 -->
    <DrawerFooter />
    
</MudDrawer>
```

**NavDrawer 规范**：

| 属性 | 桌面端 | 平板 | 手机 |
|------|--------|------|------|
| 宽度 | 256px | 256px | 100% |
| Variant | Responsive | Temporary | Temporary |
| 默认展开 | 是 | 否 | 否 |
| Overlay | 否 | 是 | 是 |

### 4.5 菜单项设计规范

```razor
<!-- 一级菜单（有子菜单） -->
<MudNavGroup Title="@L["Dashboard"]"
             Icon="@Icons.Material.Outlined.Dashboard"
             IconColor="Color.Primary"
             Expanded="@_expanded">
    
    <MudNavLink Href="/dashboard/analysis"
                Icon="@Icons.Material.Outlined.Analytics"
                Match="NavLinkMatch.All">
        @L["Analysis"]
    </MudNavLink>
    
</MudNavGroup>

<!-- 一级菜单（无子菜单） -->
<MudNavLink Href="/profile"
            Icon="@Icons.Material.Outlined.Person"
            Match="NavLinkMatch.Prefix">
    @L["Profile"]
</MudNavLink>
```

**菜单层级规范**：
- 最多支持 **3 级菜单**
- 第 1 级：带图标 + 文字
- 第 2 级：缩进 16px，小图标或无图标
- 第 3 级：缩进 32px，纯文字
- 禁止第 4 级（扁平化设计原则）

**菜单项高度**：
- 标准模式：48px（`Dense="false"`）
- 密集模式：36px（`Dense="true"`，数据管理类页面）

---

## 5. 响应式设计

### 5.1 断点系统

遵循 MudBlazor 断点（对应 Material Design）：

| 断点名 | 范围 | 设备 |
|--------|------|------|
| `Xs` | < 600px | 手机竖屏 |
| `Sm` | 600–959px | 手机横屏 / 小平板 |
| `Md` | 960–1279px | 平板 / 小笔记本 |
| `Lg` | 1280–1919px | 笔记本 / 桌面 |
| `Xl` | ≥ 1920px | 大屏显示器 |

### 5.2 网格布局系统

```razor
<!-- 统计卡片响应式示例 -->
<MudGrid Spacing="3">
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["TotalUsers"]" Value="12,345" 
                  Icon="@Icons.Material.Filled.People"
                  Color="Color.Primary" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Revenue"]" Value="¥98,765" 
                  Icon="@Icons.Material.Filled.AttachMoney"
                  Color="Color.Success" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Orders"]" Value="4,567" 
                  Icon="@Icons.Material.Filled.ShoppingCart"
                  Color="Color.Info" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Conversion"]" Value="68.5%" 
                  Icon="@Icons.Material.Filled.TrendingUp"
                  Color="Color.Warning" />
    </MudItem>
</MudGrid>
```

### 5.3 响应式行为规范

| 元素 | Desktop (≥960px) | Tablet (600-959px) | Mobile (<600px) |
|------|-----------------|-------------------|-----------------|
| Drawer | 固定展开，Persistent | 覆盖式，默认关闭 | 覆盖式，默认关闭 |
| AppBar | Dense=false，H=64 | Dense=false，H=64 | Dense=true，H=56 |
| 面包屑 | 完整显示 | 完整显示 | 最多显示2级 |
| 表格 | 完整列 | 减少列 | 卡片化展示 |
| 表单 | 多列布局 | 双列 | 单列 |
| 对话框 | MaxWidth=Small | MaxWidth=Medium | FullScreen |
| 图表 | 完整尺寸 | 自适应 | 简化版 |
| 统计卡片 | 4列 | 2列 | 1列 |

### 5.4 移动端适配规范

```razor
<!-- 表格移动端卡片化 -->
<MudHidden Breakpoint="Breakpoint.SmAndDown" Invert="true">
    <!-- 移动端：卡片列表 -->
    @foreach (var item in _items)
    {
        <MudCard Class="mb-2">
            <MudCardContent>
                <MudStack>
                    <MudText Typo="Typo.subtitle2">@item.Name</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">
                        @item.Description
                    </MudText>
                    <MudStack Row="true" Justify="Justify.SpaceBetween">
                        <MudChip Size="Size.Small">@item.Status</MudChip>
                        <MudText Typo="Typo.caption">@item.Date.ToString("yyyy-MM-dd")</MudText>
                    </MudStack>
                </MudStack>
            </MudCardContent>
            <MudCardActions>
                <MudButton Size="Size.Small">@L["Edit"]</MudButton>
                <MudButton Size="Size.Small" Color="Color.Error">@L["Delete"]</MudButton>
            </MudCardActions>
        </MudCard>
    }
</MudHidden>

<MudHidden Breakpoint="Breakpoint.SmAndDown">
    <!-- 桌面端：数据表格 -->
    <StandardDataTable Items="@_items" />
</MudHidden>
```

---

## 6. 主题系统

### 6.1 MudTheme 配置

```csharp
// Services/ThemeService.cs
public class ThemeService : IThemeService
{
    public MudTheme CurrentTheme { get; private set; } = CreateTheme();
    public bool IsDarkMode { get; private set; } = false;
    
    public event Action? OnThemeChanged;
    
    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        OnThemeChanged?.Invoke();
        // 持久化到 localStorage
    }
    
    public void SetDarkMode(bool isDark)
    {
        IsDarkMode = isDark;
        OnThemeChanged?.Invoke();
    }
    
    private static MudTheme CreateTheme() => new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            // ── 主色 ──
            Primary = "#1976D2",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#00BCD4",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#FF6F00",
            
            // ── 功能色 ──
            Success = "#4CAF50",
            Warning = "#FF9800",
            Error = "#F44336",
            Info = "#2196F3",
            
            // ── 背景 ──
            Background = "#F5F7FA",
            Surface = "#FFFFFF",
            BackgroundGray = "#F0F2F5",
            
            // ── 抽屉 ──
            DrawerBackground = "#FFFFFF",
            DrawerText = "rgba(0,0,0,0.87)",
            DrawerIcon = "rgba(0,0,0,0.54)",
            
            // ── AppBar ──
            AppbarBackground = "#FFFFFF",
            AppbarText = "rgba(0,0,0,0.87)",
            
            // ── 文字 ──
            TextPrimary = "rgba(0,0,0,0.87)",
            TextSecondary = "rgba(0,0,0,0.60)",
            TextDisabled = "rgba(0,0,0,0.38)",
            
            // ── 其他 ──
            Divider = "rgba(0,0,0,0.12)",
            ActionDefault = "rgba(0,0,0,0.54)",
            ActionDisabled = "rgba(0,0,0,0.26)",
            LinesDefault = "rgba(0,0,0,0.12)",
            TableLines = "rgba(0,0,0,0.08)",
        },
        
        PaletteDark = new PaletteDark
        {
            // ── 主色（暗色模式适当调亮）──
            Primary = "#42A5F5",
            PrimaryContrastText = "#000000",
            Secondary = "#26C6DA",
            Tertiary = "#FFA726",
            
            // ── 功能色（保持一致）──
            Success = "#66BB6A",
            Warning = "#FFA726",
            Error = "#EF5350",
            Info = "#42A5F5",
            
            // ── 背景（深色层次）──
            Background = "#111827",        // 最底层
            Surface = "#1F2937",           // 卡片层
            BackgroundGray = "#1A2332",
            
            // ── 抽屉 ──
            DrawerBackground = "#1F2937",
            DrawerText = "rgba(255,255,255,0.87)",
            DrawerIcon = "rgba(255,255,255,0.54)",
            
            // ── AppBar ──
            AppbarBackground = "#1F2937",
            AppbarText = "rgba(255,255,255,0.87)",
            
            // ── 文字 ──
            TextPrimary = "rgba(255,255,255,0.87)",
            TextSecondary = "rgba(255,255,255,0.60)",
            TextDisabled = "rgba(255,255,255,0.38)",
            
            // ── 其他 ──
            Divider = "rgba(255,255,255,0.12)",
            TableLines = "rgba(255,255,255,0.08)",
        },
        
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Roboto", "Noto Sans SC", "sans-serif" },
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.5",
            },
            H4 = new H4Typography { FontSize = "2.125rem", FontWeight = "400" },
            H5 = new H5Typography { FontSize = "1.5rem", FontWeight = "400" },
            H6 = new H6Typography { FontSize = "1.25rem", FontWeight = "500" },
            Subtitle1 = new Subtitle1Typography { FontSize = "1rem", FontWeight = "400" },
            Subtitle2 = new Subtitle2Typography { FontSize = "0.875rem", FontWeight = "500" },
            Body1 = new Body1Typography { FontSize = "1rem", FontWeight = "400" },
            Body2 = new Body2Typography { FontSize = "0.875rem", FontWeight = "400" },
            Caption = new CaptionTypography { FontSize = "0.75rem", FontWeight = "400" },
        },
        
        LayoutProperties = new LayoutProperties
        {
            DrawerWidthLeft = "256px",
            DrawerMiniWidthLeft = "56px",   // 折叠模式宽度
            AppbarHeight = "64px",
        },
        
        ZIndex = new ZIndex
        {
            Drawer = 1200,
            AppBar = 1100,
            Dialog = 1300,
            Popover = 1400,
            Snackbar = 1500,
            Tooltip = 1600,
        },
        
        Shadows = new Shadow(), // 使用 Material Design 默认阴影
        
        Shape = new ShapeConfig
        {
            BorderRadius = "8px"  // 全局圆角
        }
    };
}
```

### 6.2 主题切换组件

```razor
<!-- Shared/ThemeToggle.razor -->
<MudTooltip Text="@(ThemeService.IsDarkMode ? L["SwitchToLight"] : L["SwitchToDark"])">
    <MudIconButton 
        Icon="@(ThemeService.IsDarkMode 
               ? Icons.Material.Filled.LightMode 
               : Icons.Material.Filled.DarkMode)"
        Color="Color.Inherit"
        OnClick="@ToggleTheme" />
</MudTooltip>

@code {
    [Inject] IThemeService ThemeService { get; set; } = default!;
    
    private void ToggleTheme()
    {
        ThemeService.ToggleDarkMode();
    }
}
```

### 6.3 系统主题跟随

```javascript
// wwwroot/js/theme.js
window.adminTheme = {
    getSystemPreference: () => 
        window.matchMedia('(prefers-color-scheme: dark)').matches,
    
    listenSystemChange: (dotnetRef) => {
        const mql = window.matchMedia('(prefers-color-scheme: dark)');
        mql.addEventListener('change', (e) => {
            dotnetRef.invokeMethodAsync('OnSystemThemeChanged', e.matches);
        });
    },
    
    savePreference: (isDark) => 
        localStorage.setItem('theme', isDark ? 'dark' : 'light'),
    
    loadPreference: () => 
        localStorage.getItem('theme')
};
```

### 6.4 自定义主题扩展（品牌色）

支持运行时切换多套主题色（如不同租户品牌）：

```csharp
// 主题预设
public static class ThemePresets
{
    public static readonly ThemePreset Blue = new("Blue", "#1976D2", "#00BCD4");
    public static readonly ThemePreset Green = new("Green", "#388E3C", "#00897B");
    public static readonly ThemePreset Purple = new("Purple", "#7B1FA2", "#E91E63");
    public static readonly ThemePreset Orange = new("Orange", "#E64A19", "#FF6F00");
    
    public static IEnumerable<ThemePreset> All => 
        new[] { Blue, Green, Purple, Orange };
}

public record ThemePreset(string Name, string Primary, string Secondary);
```

---

## 7. 多语言国际化

### 7.1 架构设计

```
i18n 架构:
├── 使用 Microsoft.Extensions.Localization（官方）
├── 资源文件存储在 Resources/ 目录
├── 支持运行时切换语言（无需刷新页面）
└── 级联注入到所有组件
```

### 7.2 支持语言列表

| 代码 | 语言 | 本地名称 | 方向 |
|------|------|---------|------|
| `zh-CN` | 简体中文 | 简体中文 | LTR |
| `zh-TW` | 繁体中文 | 繁體中文 | LTR |
| `en-US` | 英语（美国） | English | LTR |
| `ja-JP` | 日语 | 日本語 | LTR |
| `ko-KR` | 韩语 | 한국어 | LTR |
| `de-DE` | 德语 | Deutsch | LTR |
| `fr-FR` | 法语 | Français | LTR |
| `ar-SA` | 阿拉伯语 | العربية | RTL |

### 7.3 资源文件结构

```
Resources/
├── Shared.resx                    ← 公共词条
├── Shared.zh-CN.resx
├── Shared.en-US.resx
├── Shared.ja-JP.resx
├── Pages/
│   ├── Dashboard.resx
│   ├── Dashboard.zh-CN.resx
│   ├── Dashboard.en-US.resx
│   ├── UserManagement.resx
│   └── UserManagement.zh-CN.resx
└── Components/
    ├── DataTable.resx
    └── DataTable.zh-CN.resx
```

### 7.4 资源文件规范（Shared.resx）

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- ── 通用操作 ── -->
  <data name="Common.Save" xml:space="preserve"><value>保存</value></data>
  <data name="Common.Cancel" xml:space="preserve"><value>取消</value></data>
  <data name="Common.Delete" xml:space="preserve"><value>删除</value></data>
  <data name="Common.Edit" xml:space="preserve"><value>编辑</value></data>
  <data name="Common.Create" xml:space="preserve"><value>新建</value></data>
  <data name="Common.Search" xml:space="preserve"><value>搜索</value></data>
  <data name="Common.Reset" xml:space="preserve"><value>重置</value></data>
  <data name="Common.Confirm" xml:space="preserve"><value>确认</value></data>
  <data name="Common.View" xml:space="preserve"><value>查看</value></data>
  <data name="Common.Export" xml:space="preserve"><value>导出</value></data>
  <data name="Common.Import" xml:space="preserve"><value>导入</value></data>
  
  <!-- ── 状态 ── -->
  <data name="Status.Active" xml:space="preserve"><value>启用</value></data>
  <data name="Status.Inactive" xml:space="preserve"><value>禁用</value></data>
  <data name="Status.Pending" xml:space="preserve"><value>待处理</value></data>
  <data name="Status.Processing" xml:space="preserve"><value>处理中</value></data>
  <data name="Status.Completed" xml:space="preserve"><value>已完成</value></data>
  <data name="Status.Failed" xml:space="preserve"><value>失败</value></data>
  
  <!-- ── 提示消息 ── -->
  <data name="Message.SaveSuccess" xml:space="preserve"><value>保存成功</value></data>
  <data name="Message.DeleteSuccess" xml:space="preserve"><value>删除成功</value></data>
  <data name="Message.DeleteConfirm" xml:space="preserve"><value>确定要删除此记录吗？此操作不可撤销。</value></data>
  <data name="Message.NetworkError" xml:space="preserve"><value>网络请求失败，请稍后重试</value></data>
  <data name="Message.Loading" xml:space="preserve"><value>加载中...</value></data>
  <data name="Message.NoData" xml:space="preserve"><value>暂无数据</value></data>
  
  <!-- ── 验证 ── -->
  <data name="Validation.Required" xml:space="preserve"><value>{0} 为必填项</value></data>
  <data name="Validation.MaxLength" xml:space="preserve"><value>{0} 最多 {1} 个字符</value></data>
  <data name="Validation.Email" xml:space="preserve"><value>请输入有效的邮箱地址</value></data>
  
  <!-- ── 分页 ── -->
  <data name="Pagination.Total" xml:space="preserve"><value>共 {0} 条</value></data>
  <data name="Pagination.RowsPerPage" xml:space="preserve"><value>每页行数</value></data>
  
  <!-- ── 导航菜单 ── -->
  <data name="Menu.Dashboard" xml:space="preserve"><value>仪表板</value></data>
  <data name="Menu.UserManagement" xml:space="preserve"><value>用户管理</value></data>
  <data name="Menu.RoleManagement" xml:space="preserve"><value>角色管理</value></data>
  <data name="Menu.SystemSettings" xml:space="preserve"><value>系统设置</value></data>
  <data name="Menu.AuditLog" xml:space="preserve"><value>审计日志</value></data>
</root>
```

### 7.5 语言服务实现

```csharp
// Services/LanguageService.cs
public class LanguageService : ILanguageService
{
    private readonly NavigationManager _nav;
    private readonly IJSRuntime _js;
    
    public CultureInfo CurrentCulture { get; private set; } = 
        new CultureInfo("zh-CN");
    
    public static readonly IReadOnlyList<LanguageOption> SupportedLanguages = 
        new List<LanguageOption>
        {
            new("zh-CN", "简体中文", "🇨🇳"),
            new("zh-TW", "繁體中文", "🇹🇼"),
            new("en-US", "English",  "🇺🇸"),
            new("ja-JP", "日本語",   "🇯🇵"),
            new("ko-KR", "한국어",   "🇰🇷"),
        };
    
    public async Task SetLanguageAsync(string culture)
    {
        // 持久化
        await _js.InvokeVoidAsync("localStorage.setItem", "culture", culture);
        
        // 刷新应用以应用新语言
        // Blazor Server: 通过 CultureInfo.CurrentCulture 设置
        // Blazor WASM: 通过导航刷新
        _nav.NavigateTo(_nav.Uri, forceLoad: true);
    }
}

public record LanguageOption(string Code, string DisplayName, string Flag);
```

### 7.6 组件内使用

```razor
@* 在每个组件中注入 *@
@inject IStringLocalizer<Shared> L

@* 使用 *@
<MudButton>@L["Common.Save"]</MudButton>
<MudText>@string.Format(L["Validation.Required"], L["Common.Name"])</MudText>
```

### 7.7 语言切换组件

```razor
<!-- Shared/LanguageSwitcher.razor -->
<MudMenu Icon="@Icons.Material.Outlined.Language" 
         Color="Color.Inherit"
         AnchorOrigin="Origin.BottomRight"
         TransformOrigin="Origin.TopRight">
    @foreach (var lang in LanguageService.SupportedLanguages)
    {
        <MudMenuItem OnClick="@(() => SetLanguage(lang.Code))">
            <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                <MudText>@lang.Flag</MudText>
                <MudText>@lang.DisplayName</MudText>
                @if (lang.Code == CurrentCulture)
                {
                    <MudIcon Icon="@Icons.Material.Filled.Check" 
                             Size="Size.Small" 
                             Color="Color.Primary" />
                }
            </MudStack>
        </MudMenuItem>
    }
</MudMenu>
```

### 7.8 RTL 支持

```csharp
// 在 MainLayout 中检测 RTL
protected override void OnParametersSet()
{
    var culture = CultureInfo.CurrentCulture;
    var isRtl = culture.TextInfo.IsRightToLeft;
    // 通过 JS 设置 dir 属性
}
```

```javascript
// wwwroot/js/rtl.js
window.setDirection = (isRtl) => {
    document.documentElement.setAttribute('dir', isRtl ? 'rtl' : 'ltr');
    document.documentElement.setAttribute('lang', 
        document.documentElement.lang);
};
```

---

## 8. 路由与导航

### 8.1 路由规范

```
路由命名规范：
/                         → 重定向到 /dashboard/analysis
/dashboard/analysis       → 分析概览
/dashboard/monitor        → 监控大屏
/system/users             → 用户列表
/system/users/create      → 新建用户
/system/users/{id}/edit   → 编辑用户
/system/users/{id}        → 用户详情
/system/roles             → 角色管理
/system/menus             → 菜单管理
/system/settings          → 系统设置
/profile                  → 个人资料
/account/login            → 登录（不含 MainLayout）
/account/register         → 注册
/error/403                → 无权限
/error/404                → 找不到
/error/500                → 服务器错误
```

### 8.2 路由配置

```csharp
// App.razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" 
                            DefaultLayout="@typeof(MainLayout)">
            <NotAuthorized>
                <RedirectToLogin />
            </NotAuthorized>
            <Authorizing>
                <LoadingPage />
            </Authorizing>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <NotFoundPage />
        </LayoutView>
    </NotFound>
</Router>
```

### 8.3 面包屑导航

```csharp
// Services/BreadcrumbService.cs
public class BreadcrumbService
{
    public List<BreadcrumbItem> Items { get; private set; } = new();
    public event Action? OnChanged;
    
    public void SetBreadcrumbs(params BreadcrumbItem[] items)
    {
        Items = items.ToList();
        OnChanged?.Invoke();
    }
}
```

```razor
<!-- 页面中使用 -->
@code {
    protected override void OnInitialized()
    {
        BreadcrumbService.SetBreadcrumbs(
            new BreadcrumbItem(L["Menu.Dashboard"], href: "/"),
            new BreadcrumbItem(L["Menu.UserManagement"], href: "/system/users"),
            new BreadcrumbItem(L["Common.Edit"], href: null, disabled: true)
        );
    }
}
```

---

## 9. 组件规范

### 9.1 页面容器组件

```razor
<!-- Components/PageContainer.razor -->
@* 标准页面容器，包含标题区域和内容区域 *@

<div class="page-container">
    <!-- 页面头部 -->
    <div class="page-header mb-4">
        <MudStack Row="true" AlignItems="AlignItems.Center" 
                  Justify="Justify.SpaceBetween" Wrap="Wrap.Wrap">
            <div>
                <MudText Typo="Typo.h5" GutterBottom="false">@Title</MudText>
                @if (!string.IsNullOrEmpty(Description))
                {
                    <MudText Typo="Typo.body2" Color="Color.Secondary">
                        @Description
                    </MudText>
                }
            </div>
            <MudStack Row="true" Spacing="2">
                @HeaderActions
            </MudStack>
        </MudStack>
    </div>
    
    <!-- 页面内容 -->
    <div class="page-content">
        @ChildContent
    </div>
</div>

@code {
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string? Description { get; set; }
    [Parameter] public RenderFragment? HeaderActions { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

### 9.2 搜索查询栏组件

```razor
<!-- Components/SearchPanel.razor -->
<MudPaper Elevation="0" Outlined="true" Class="pa-4 mb-4 rounded-lg">
    <MudGrid Spacing="2">
        @ChildContent
        
        <!-- 操作按钮 -->
        <MudItem xs="12" Class="d-flex justify-end gap-2">
            <MudButton Variant="Variant.Outlined" 
                       StartIcon="@Icons.Material.Outlined.Refresh"
                       OnClick="@OnReset">
                @L["Common.Reset"]
            </MudButton>
            <MudButton Variant="Variant.Filled" 
                       Color="Color.Primary"
                       StartIcon="@Icons.Material.Outlined.Search"
                       OnClick="@OnSearch">
                @L["Common.Search"]
            </MudButton>
        </MudItem>
    </MudGrid>
</MudPaper>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnSearch { get; set; }
    [Parameter] public EventCallback OnReset { get; set; }
}
```

### 9.3 标准数据表格组件

```razor
<!-- Components/StandardDataTable.razor -->
<MudCard Elevation="0" Outlined="true" Class="rounded-lg">
    <!-- 表格工具栏 -->
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.subtitle1" Style="font-weight:500">
                @Title
            </MudText>
        </CardHeaderContent>
        <CardHeaderActions>
            <MudStack Row="true" Spacing="1">
                @ToolbarActions
                <MudIconButton Icon="@Icons.Material.Outlined.FileDownload"
                               Size="Size.Small"
                               Tooltip="@L["Common.Export"]"
                               OnClick="@OnExport" />
                <MudIconButton Icon="@Icons.Material.Outlined.Refresh"
                               Size="Size.Small"
                               OnClick="@OnRefresh" />
            </MudStack>
        </CardHeaderActions>
    </MudCardHeader>
    
    <!-- 批量操作栏（勾选时显示） -->
    @if (_selectedItems.Any())
    {
        <MudAlert Severity="Severity.Info" Class="mx-4 mb-2 rounded">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudText>@string.Format(L["Table.Selected"], _selectedItems.Count)</MudText>
                <MudSpacer />
                <MudButton Size="Size.Small" OnClick="@OnBatchDelete" 
                           Color="Color.Error">
                    @L["Common.BatchDelete"]
                </MudButton>
            </MudStack>
        </MudAlert>
    }
    
    <!-- 数据表格 -->
    <MudDataGrid T="TItem"
                 Items="@Items"
                 ServerData="@ServerData"
                 MultiSelection="true"
                 @bind-SelectedItems="_selectedItems"
                 Filterable="false"
                 SortMode="SortMode.Multiple"
                 Loading="@Loading"
                 LoadingProgressColor="Color.Primary"
                 RowsPerPage="20"
                 Striped="true"
                 Hover="true"
                 Dense="@Dense"
                 Breakpoint="Breakpoint.Sm">
        
        @Columns
        
        <!-- 操作列（固定右侧） -->
        <TemplateColumn CellClass="d-flex justify-end" Sticky="true" 
                        Title="@L["Common.Actions"]" 
                        HeaderStyle="text-align:right;">
            <CellTemplate>
                @ActionsTemplate?.Invoke(context.Item)
            </CellTemplate>
        </TemplateColumn>
        
        <!-- 空数据提示 -->
        <NoRecordsContent>
            <EmptyState Icon="@Icons.Material.Outlined.Inbox"
                        Description="@L["Message.NoData"]" />
        </NoRecordsContent>
        
    </MudDataGrid>
</MudCard>

@code {
    [Parameter] public string? Title { get; set; }
    [Parameter] public IEnumerable<TItem>? Items { get; set; }
    [Parameter] public Func<GridState<TItem>, Task<GridData<TItem>>>? ServerData { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool Dense { get; set; }
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public RenderFragment<TItem>? ActionsTemplate { get; set; }
    [Parameter] public RenderFragment? ToolbarActions { get; set; }
    [Parameter] public EventCallback OnExport { get; set; }
    [Parameter] public EventCallback OnRefresh { get; set; }
    [Parameter] public EventCallback OnBatchDelete { get; set; }
    
    private HashSet<TItem> _selectedItems = new();
}
```

### 9.4 统计卡片组件

```razor
<!-- Components/StatCard.razor -->
<MudCard Elevation="0" Outlined="true" Class="stat-card rounded-lg h-100">
    <MudCardContent Class="pa-6">
        <MudStack Row="true" Justify="Justify.SpaceBetween" 
                  AlignItems="AlignItems.FlexStart">
            <div class="flex-1">
                <MudText Typo="Typo.body2" Color="Color.Secondary" 
                         GutterBottom="false">
                    @Title
                </MudText>
                <MudText Typo="Typo.h4" Style="font-weight:600; line-height:1.2"
                         Class="my-1">
                    @Value
                </MudText>
                @if (Trend.HasValue)
                {
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="1">
                        <MudIcon Icon="@(Trend > 0 
                                       ? Icons.Material.Filled.TrendingUp 
                                       : Icons.Material.Filled.TrendingDown)"
                                 Size="Size.Small"
                                 Color="@(Trend > 0 ? Color.Success : Color.Error)" />
                        <MudText Typo="Typo.caption"
                                 Color="@(Trend > 0 ? Color.Success : Color.Error)">
                            @(Trend > 0 ? "+" : "")@Trend%
                        </MudText>
                        <MudText Typo="Typo.caption" Color="Color.Secondary">
                            @L["vs. last month"]
                        </MudText>
                    </MudStack>
                }
            </div>
            <MudAvatar Color="@Color" Rounded="true" 
                       Style="width:48px;height:48px;">
                <MudIcon Icon="@Icon" Size="Size.Medium" />
            </MudAvatar>
        </MudStack>
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = string.Empty;
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public decimal? Trend { get; set; }
}
```

### 9.5 表单对话框组件

```razor
<!-- Components/FormDialog.razor -->
<MudDialog>
    <TitleContent>
        <MudText Typo="Typo.h6">@Title</MudText>
    </TitleContent>
    
    <DialogContent>
        <MudForm @ref="_form" Model="@Model" Validation="@Validator.ValidateValue">
            @FormContent
        </MudForm>
    </DialogContent>
    
    <DialogActions>
        <MudButton OnClick="@Cancel" 
                   Variant="Variant.Text">
            @L["Common.Cancel"]
        </MudButton>
        <MudButton OnClick="@Submit" 
                   Color="Color.Primary" 
                   Variant="Variant.Filled"
                   Disabled="@_submitting">
            @if (_submitting)
            {
                <MudProgressCircular Size="Size.Small" Indeterminate="true" 
                                     Class="mr-2" />
            }
            @L["Common.Save"]
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public object? Model { get; set; }
    [Parameter] public object? Validator { get; set; }
    [Parameter] public RenderFragment? FormContent { get; set; }
    [Parameter] public Func<Task<bool>>? OnSubmit { get; set; }
    
    private MudForm _form = default!;
    private bool _submitting;
    
    private void Cancel() => MudDialog.Cancel();
    
    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid) return;
        
        _submitting = true;
        try
        {
            var success = OnSubmit != null && await OnSubmit();
            if (success) MudDialog.Close(DialogResult.Ok(true));
        }
        finally
        {
            _submitting = false;
        }
    }
}
```

### 9.6 空状态组件

```razor
<!-- Components/EmptyState.razor -->
<div class="d-flex flex-column align-center justify-center pa-8">
    <MudIcon Icon="@Icon" 
             Style="font-size: 64px; opacity: 0.3;" 
             Color="Color.Default"
             Class="mb-4" />
    <MudText Typo="Typo.subtitle1" Color="Color.Secondary" 
             GutterBottom="true">
        @Description
    </MudText>
    @if (ActionContent != null)
    {
        <div class="mt-2">@ActionContent</div>
    }
</div>

@code {
    [Parameter] public string Icon { get; set; } = Icons.Material.Outlined.Inbox;
    [Parameter] public string Description { get; set; } = "暂无数据";
    [Parameter] public RenderFragment? ActionContent { get; set; }
}
```

### 9.7 确认删除对话框

```razor
<!-- Components/ConfirmDeleteDialog.razor -->
<MudDialog>
    <TitleContent>
        <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
            <MudIcon Icon="@Icons.Material.Filled.Warning" Color="Color.Warning" />
            <MudText Typo="Typo.h6">@L["Common.DeleteConfirmTitle"]</MudText>
        </MudStack>
    </TitleContent>
    
    <DialogContent>
        <MudText>@Message</MudText>
        @if (ItemName != null)
        {
            <MudAlert Severity="Severity.Warning" Class="mt-3">
                <strong>@ItemName</strong>
            </MudAlert>
        }
    </DialogContent>
    
    <DialogActions>
        <MudButton OnClick="@Cancel">@L["Common.Cancel"]</MudButton>
        <MudButton Color="Color.Error" Variant="Variant.Filled" 
                   OnClick="@Confirm">
            @L["Common.Delete"]
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public string Message { get; set; } = string.Empty;
    [Parameter] public string? ItemName { get; set; }
    
    private void Cancel() => MudDialog.Cancel();
    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));
}
```

### 9.8 通知消息规范

```csharp
// Services/NotificationService.cs
public class NotificationService
{
    private readonly ISnackbar _snackbar;
    
    public void Success(string message) =>
        _snackbar.Add(message, Severity.Success, 
            c => { c.VisibleStateDuration = 3000; });
    
    public void Error(string message) =>
        _snackbar.Add(message, Severity.Error,
            c => { c.VisibleStateDuration = 5000; c.RequireInteraction = true; });
    
    public void Warning(string message) =>
        _snackbar.Add(message, Severity.Warning,
            c => { c.VisibleStateDuration = 4000; });
    
    public void Info(string message) =>
        _snackbar.Add(message, Severity.Info,
            c => { c.VisibleStateDuration = 3000; });
}
```

**Snackbar 位置规范**：
- 位置：右下角（`SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight`）
- 最大显示数量：3 条
- 成功消息：3 秒自动消失
- 错误消息：5 秒，需手动关闭

---

## 10. 页面模板

### 10.1 Dashboard 分析页

```razor
<!-- Pages/Dashboard/Analysis.razor -->
@page "/dashboard/analysis"
@attribute [Authorize]

<PageContainer Title="@L["Menu.Dashboard.Analysis"]"
               Description="@L["Dashboard.Description"]">
    <HeaderActions>
        <MudSelect T="string" Value="@_period" Dense="true" 
                   Style="min-width:120px;">
            <MudSelectItem Value="@("today")">@L["Period.Today"]</MudSelectItem>
            <MudSelectItem Value="@("week")">@L["Period.Week"]</MudSelectItem>
            <MudSelectItem Value="@("month")">@L["Period.Month"]</MudSelectItem>
        </MudSelect>
        <MudButton Variant="Variant.Outlined" Size="Size.Small"
                   StartIcon="@Icons.Material.Outlined.FileDownload">
            @L["Common.Export"]
        </MudButton>
    </HeaderActions>
</PageContainer>

<!-- 统计卡片行 -->
<MudGrid Spacing="3" Class="mb-6">
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Stats.TotalUsers"]" Value="12,845" 
                  Icon="@Icons.Material.Outlined.People"
                  Color="Color.Primary" Trend="12.5" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Stats.Revenue"]" Value="¥ 98,765" 
                  Icon="@Icons.Material.Outlined.AccountBalanceWallet"
                  Color="Color.Success" Trend="8.2" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Stats.Orders"]" Value="4,567" 
                  Icon="@Icons.Material.Outlined.ShoppingCart"
                  Color="Color.Warning" Trend="-2.1" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <StatCard Title="@L["Stats.Conversion"]" Value="68.5%" 
                  Icon="@Icons.Material.Outlined.TrendingUp"
                  Color="Color.Info" Trend="3.8" />
    </MudItem>
</MudGrid>

<!-- 图表区域 -->
<MudGrid Spacing="3" Class="mb-6">
    <MudItem xs="12" md="8">
        <MudCard Elevation="0" Outlined="true" Class="rounded-lg h-100">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.subtitle1" Style="font-weight:500">
                        @L["Chart.SalesTrend"]
                    </MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <!-- 插入图表组件 -->
                <SalesTrendChart Data="@_salesData" Height="300" />
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" md="4">
        <MudCard Elevation="0" Outlined="true" Class="rounded-lg h-100">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.subtitle1" Style="font-weight:500">
                        @L["Chart.CategoryDistribution"]
                    </MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <CategoryPieChart Data="@_categoryData" Height="300" />
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>

<!-- 数据表格 -->
<MudGrid Spacing="3">
    <MudItem xs="12" md="8">
        <RecentOrdersTable />
    </MudItem>
    <MudItem xs="12" md="4">
        <TopProductsList />
    </MudItem>
</MudGrid>
```

### 10.2 标准列表页模板

```razor
<!-- Pages/System/Users/Index.razor -->
@page "/system/users"
@attribute [Authorize(Roles = "Admin,UserManager")]

<PageContainer Title="@L["Menu.UserManagement"]">
    <HeaderActions>
        <MudButton Variant="Variant.Filled" 
                   Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="@OpenCreateDialog">
            @L["Common.Create"]
        </MudButton>
    </HeaderActions>
</PageContainer>

<!-- 搜索区域 -->
<SearchPanel OnSearch="@Search" OnReset="@Reset">
    <MudItem xs="12" sm="6" md="3">
        <MudTextField @bind-Value="_query.Name" 
                      Label="@L["User.Name"]"
                      Variant="Variant.Outlined"
                      Clearable="true"
                      Dense="true" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudTextField @bind-Value="_query.Email" 
                      Label="@L["User.Email"]"
                      Variant="Variant.Outlined"
                      Clearable="true"
                      Dense="true" />
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudSelect @bind-Value="_query.Status" 
                   Label="@L["Common.Status"]"
                   Variant="Variant.Outlined"
                   Clearable="true"
                   Dense="true">
            <MudSelectItem Value="@(1)">@L["Status.Active"]</MudSelectItem>
            <MudSelectItem Value="@(0)">@L["Status.Inactive"]</MudSelectItem>
        </MudSelect>
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudDateRangePicker @bind-DateRange="_query.DateRange"
                            Label="@L["Common.DateRange"]"
                            Variant="Variant.Outlined"
                            Dense="true" />
    </MudItem>
</SearchPanel>

<!-- 数据表格 -->
<StandardDataTable T="UserDto"
                   Title="@L["Menu.UserManagement"]"
                   ServerData="@LoadServerData"
                   Loading="@_loading"
                   OnRefresh="@Refresh"
                   OnExport="@Export">
    <Columns>
        <PropertyColumn Property="x => x.Avatar" Title="" Sortable="false">
            <CellTemplate>
                <MudAvatar Size="Size.Small">
                    @context.Item.Name[0]
                </MudAvatar>
            </CellTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.Name" 
                        Title="@L["User.Name"]" Sortable="true" />
        <PropertyColumn Property="x => x.Email" 
                        Title="@L["User.Email"]" />
        <PropertyColumn Property="x => x.Role" 
                        Title="@L["User.Role"]">
            <CellTemplate>
                <MudChip Size="Size.Small" Color="Color.Primary" 
                         Variant="Variant.Outlined">
                    @context.Item.Role
                </MudChip>
            </CellTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.Status" 
                        Title="@L["Common.Status"]">
            <CellTemplate>
                <MudChip Size="Size.Small" 
                         Color="@(context.Item.Status == 1 
                                 ? Color.Success : Color.Default)">
                    @(context.Item.Status == 1 
                      ? L["Status.Active"] : L["Status.Inactive"])
                </MudChip>
            </CellTemplate>
        </PropertyColumn>
        <PropertyColumn Property="x => x.CreatedAt" 
                        Title="@L["Common.CreatedAt"]"
                        Format="yyyy-MM-dd" />
    </Columns>
    <ActionsTemplate Context="item">
        <MudIconButton Icon="@Icons.Material.Outlined.Visibility" 
                       Size="Size.Small"
                       OnClick="@(() => ViewDetail(item))"
                       Tooltip="@L["Common.View"]" />
        <MudIconButton Icon="@Icons.Material.Outlined.Edit" 
                       Size="Size.Small"
                       OnClick="@(() => OpenEditDialog(item))"
                       Tooltip="@L["Common.Edit"]" />
        <MudIconButton Icon="@Icons.Material.Outlined.Delete" 
                       Size="Size.Small"
                       Color="Color.Error"
                       OnClick="@(() => ConfirmDelete(item))"
                       Tooltip="@L["Common.Delete"]" />
    </ActionsTemplate>
</StandardDataTable>
```

### 10.3 表单页面模板

```razor
<!-- Pages/System/Users/Create.razor -->
@page "/system/users/create"
@attribute [Authorize(Roles = "Admin")]

<PageContainer Title="@L["User.CreateTitle"]">
    <HeaderActions>
        <MudButton Variant="Variant.Outlined"
                   StartIcon="@Icons.Material.Outlined.ArrowBack"
                   OnClick="@GoBack">
            @L["Common.Back"]
        </MudButton>
    </HeaderActions>
</PageContainer>

<MudGrid Spacing="3">
    <!-- 主表单 -->
    <MudItem xs="12" md="8">
        <MudCard Elevation="0" Outlined="true" Class="rounded-lg">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.subtitle1" Style="font-weight:500">
                        @L["User.BasicInfo"]
                    </MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <MudForm @ref="_form" Model="@_model" 
                         Validation="@(_validator.ValidateValue)">
                    <MudGrid Spacing="3">
                        <MudItem xs="12" sm="6">
                            <MudTextField @bind-Value="_model.FirstName"
                                          For="@(() => _model.FirstName)"
                                          Label="@L["User.FirstName"]"
                                          Variant="Variant.Outlined"
                                          Required="true" />
                        </MudItem>
                        <MudItem xs="12" sm="6">
                            <MudTextField @bind-Value="_model.LastName"
                                          For="@(() => _model.LastName)"
                                          Label="@L["User.LastName"]"
                                          Variant="Variant.Outlined"
                                          Required="true" />
                        </MudItem>
                        <MudItem xs="12">
                            <MudTextField @bind-Value="_model.Email"
                                          For="@(() => _model.Email)"
                                          Label="@L["User.Email"]"
                                          Variant="Variant.Outlined"
                                          InputType="InputType.Email"
                                          Required="true" />
                        </MudItem>
                        <MudItem xs="12" sm="6">
                            <MudSelect @bind-Value="_model.RoleId"
                                       For="@(() => _model.RoleId)"
                                       Label="@L["User.Role"]"
                                       Variant="Variant.Outlined"
                                       Required="true">
                                @foreach (var role in _roles)
                                {
                                    <MudSelectItem Value="@role.Id">
                                        @role.Name
                                    </MudSelectItem>
                                }
                            </MudSelect>
                        </MudItem>
                        <MudItem xs="12" sm="6">
                            <MudSwitch @bind-Checked="_model.IsActive"
                                       Label="@L["User.IsActive"]"
                                       Color="Color.Primary" />
                        </MudItem>
                    </MudGrid>
                </MudForm>
            </MudCardContent>
            <MudCardActions Class="pa-4 d-flex justify-end gap-2">
                <MudButton Variant="Variant.Text" OnClick="@Reset">
                    @L["Common.Reset"]
                </MudButton>
                <MudButton Variant="Variant.Filled" 
                           Color="Color.Primary"
                           Disabled="@_submitting"
                           OnClick="@Submit">
                    @if (_submitting)
                    {
                        <MudProgressCircular Size="Size.Small" 
                                             Indeterminate="true" 
                                             Class="mr-2" />
                    }
                    @L["Common.Save"]
                </MudButton>
            </MudCardActions>
        </MudCard>
    </MudItem>
    
    <!-- 右侧辅助信息 -->
    <MudItem xs="12" md="4">
        <MudCard Elevation="0" Outlined="true" Class="rounded-lg">
            <MudCardHeader>
                <CardHeaderContent>
                    <MudText Typo="Typo.subtitle1" Style="font-weight:500">
                        @L["Common.Tips"]
                    </MudText>
                </CardHeaderContent>
            </MudCardHeader>
            <MudCardContent>
                <MudList Dense="true" DisablePadding="true">
                    <MudListItem Icon="@Icons.Material.Outlined.Info"
                                 IconColor="Color.Info">
                        @L["User.CreateTip1"]
                    </MudListItem>
                    <MudListItem Icon="@Icons.Material.Outlined.Info"
                                 IconColor="Color.Info">
                        @L["User.CreateTip2"]
                    </MudListItem>
                </MudList>
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>
```

---

## 11. Blazor 双模式支持

### 11.1 模式对比

| 特性 | Blazor Server | Blazor WebAssembly |
|------|--------------|-------------------|
| 渲染位置 | 服务器端 | 客户端 |
| 首次加载 | 快 | 较慢（需下载 WASM） |
| 离线支持 | ❌ | ✅（PWA 模式） |
| 实时功能 | ✅ (SignalR) | 需 WebSocket |
| SEO | ✅ | 需 Prerendering |
| 内存占用 | 服务器内存 | 客户端内存 |
| 适用场景 | 内部系统 | 公网产品 |
| 数据库直连 | ✅ | ❌（需 API） |

### 11.2 项目结构（统一代码库）

```
Solution/
├── AdminPro.Shared/            ← 共享代码
│   ├── Models/
│   ├── Services/Interfaces/
│   ├── Components/             ← 所有 Razor 组件
│   ├── Pages/
│   ├── Layouts/
│   └── Resources/              ← i18n 资源文件
│
├── AdminPro.Server/            ← Blazor Server 宿主
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Services/               ← Server 特有服务实现
│   └── Hubs/                   ← SignalR Hubs
│
├── AdminPro.Client/            ← Blazor WASM 宿主
│   ├── Program.cs
│   ├── wwwroot/
│   └── Services/               ← WASM 特有服务实现（API 调用）
│
└── AdminPro.Api/               ← ASP.NET Core API（WASM 模式使用）
    ├── Controllers/
    ├── Program.cs
    └── appsettings.json
```

### 11.3 Blazor Server 配置

```csharp
// AdminPro.Server/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = 
        Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
});

// 多语言
builder.Services.AddLocalization(options => 
    options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] 
    { 
        "zh-CN", "zh-TW", "en-US", "ja-JP", "ko-KR" 
    };
    options.SetDefaultCulture("zh-CN")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// 认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(/* JWT 配置 */);

// 业务服务
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// SignalR（实时功能）
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<NotificationHub>("/hubs/notification");

app.Run();
```

### 11.4 Blazor WebAssembly 配置

```csharp
// AdminPro.Client/Program.cs
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// MudBlazor
builder.Services.AddMudServices(/* 同 Server 配置 */);

// HTTP 客户端（指向 API）
builder.Services.AddHttpClient("AdminApi", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthTokenHandler>();

// 多语言（WASM 方式）
builder.Services.AddLocalization();

// 认证（WASM）
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth", options.ProviderOptions);
});

// 状态管理
builder.Services.AddFluxor(o => o
    .ScanAssemblies(typeof(Program).Assembly)
    .UseReduxDevTools());

// 业务服务（WASM 版本）
builder.Services.AddScoped<IThemeService, WasmThemeService>();
builder.Services.AddScoped<IUserService, ApiUserService>();

await builder.Build().RunAsync();
```

### 11.5 服务抽象（跨模式共享）

```csharp
// Shared/Services/Interfaces/IUserService.cs
public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query);
    Task<UserDto?> GetUserAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task DeleteUserAsync(Guid id);
}

// Server 实现（直接访问数据库）
public class DbUserService : IUserService { ... }

// WASM 实现（通过 HTTP API）
public class ApiUserService : IUserService 
{
    private readonly HttpClient _http;
    
    public async Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query)
    {
        return await _http.GetFromJsonAsync<PagedResult<UserDto>>(
            $"api/users?{query.ToQueryString()}")
            ?? new PagedResult<UserDto>();
    }
}
```

### 11.6 渲染模式切换（.NET 8 Auto 模式）

```csharp
// .NET 8 支持 Auto 模式（自动选择 Server / WASM）
// App.razor
<Routes @rendermode="InteractiveAuto" />

// 页面级别指定
@rendermode InteractiveServer   // 强制 Server
@rendermode InteractiveWebAssembly  // 强制 WASM
@rendermode InteractiveAuto     // 自动选择
```

---

## 12. 项目结构

### 12.1 完整目录结构

```
AdminPro.Shared/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   ├── AuthLayout.razor
│   │   ├── AppBar.razor
│   │   ├── NavDrawer.razor
│   │   ├── BreadcrumbNav.razor
│   │   └── DrawerFooter.razor
│   │
│   ├── Shared/
│   │   ├── ThemeToggle.razor
│   │   ├── LanguageSwitcher.razor
│   │   ├── NotificationBell.razor
│   │   ├── UserAvatar.razor
│   │   ├── GlobalSearch.razor
│   │   └── FullscreenToggle.razor
│   │
│   ├── Common/
│   │   ├── PageContainer.razor
│   │   ├── SearchPanel.razor
│   │   ├── StandardDataTable.razor
│   │   ├── StatCard.razor
│   │   ├── FormDialog.razor
│   │   ├── ConfirmDeleteDialog.razor
│   │   ├── EmptyState.razor
│   │   ├── LoadingOverlay.razor
│   │   └── StatusChip.razor
│   │
│   └── Charts/
│       ├── LineChart.razor
│       ├── BarChart.razor
│       ├── PieChart.razor
│       └── AreaChart.razor
│
├── Pages/
│   ├── Dashboard/
│   │   ├── Analysis.razor
│   │   └── Monitor.razor
│   ├── System/
│   │   ├── Users/
│   │   │   ├── Index.razor
│   │   │   ├── Detail.razor
│   │   │   ├── Create.razor
│   │   │   └── Edit.razor
│   │   ├── Roles/
│   │   │   └── Index.razor
│   │   ├── Menus/
│   │   │   └── Index.razor
│   │   └── Settings/
│   │       └── Index.razor
│   ├── Profile/
│   │   └── Index.razor
│   ├── Account/
│   │   ├── Login.razor
│   │   └── Register.razor
│   └── Error/
│       ├── NotFound.razor
│       ├── Forbidden.razor
│       └── ServerError.razor
│
├── Services/
│   ├── Interfaces/
│   │   ├── IThemeService.cs
│   │   ├── ILanguageService.cs
│   │   ├── IBreadcrumbService.cs
│   │   └── IUserService.cs
│   └── Implementations/
│       ├── ThemeService.cs
│       └── BreadcrumbService.cs
│
├── Models/
│   ├── DTOs/
│   ├── ViewModels/
│   └── Enums/
│
├── Validators/
│   ├── UserValidator.cs
│   └── RoleValidator.cs
│
├── Helpers/
│   ├── CultureHelper.cs
│   └── PermissionHelper.cs
│
├── Constants/
│   ├── AppConstants.cs
│   └── RouteConstants.cs
│
└── Resources/
    ├── Shared.resx
    ├── Shared.zh-CN.resx
    ├── Shared.en-US.resx
    ├── Shared.ja-JP.resx
    └── Pages/
        ├── Dashboard.resx
        ├── Dashboard.zh-CN.resx
        └── ...
```

---

## 13. 开发规范

### 13.1 命名规范

```
组件命名:
  ✅ UserCreateDialog.razor      (PascalCase)
  ✅ StandardDataTable.razor
  ❌ userCreateDialog.razor
  ❌ user_create_dialog.razor

页面路由:
  ✅ @page "/system/users"       (kebab-case)
  ✅ @page "/system/users/{id:guid}/edit"
  ❌ @page "/System/Users"
  ❌ @page "/systemUsers"

CSS 类名:
  ✅ page-container
  ✅ stat-card__title
  ❌ pageContainer
  ❌ PageContainer

服务接口:
  ✅ IUserService
  ✅ IThemeService
  ❌ UserService（接口用 I 前缀）

方法命名:
  ✅ GetUsersAsync（异步方法 Async 后缀）
  ✅ HandleSearchAsync
  ❌ GetUsers（异步方法缺 Async）
```

### 13.2 组件开发规范

```razor
<!-- 标准组件模板 -->
@* 1. 使用 @inject 而非构造函数注入（Blazor 推荐）*@
@inject IStringLocalizer<Shared> L
@inject IUserService UserService
@inject ISnackbar Snackbar

@* 2. 实现 IAsyncDisposable 而非 IDisposable *@
@implements IAsyncDisposable

<div><!-- 组件内容 --></div>

@code {
    // 3. 参数在前
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnValueChanged { get; set; }
    
    // 4. 私有字段小写 _camelCase
    private bool _loading;
    private List<UserDto> _users = new();
    
    // 5. 生命周期：只在 OnInitializedAsync 中加载数据
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }
    
    // 6. 所有 IO 操作必须 try-catch
    private async Task LoadDataAsync()
    {
        _loading = true;
        try
        {
            _users = await UserService.GetUsersAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }
    
    // 7. 实现资源释放
    public async ValueTask DisposeAsync()
    {
        // 取消订阅事件、释放资源
    }
}
```

### 13.3 表单验证规范

```csharp
// 使用 FluentValidation
public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator(IStringLocalizer<Shared> L)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(L["Validation.Required", "姓名"])
            .MaximumLength(50).WithMessage(L["Validation.MaxLength", "姓名", 50]);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage(L["Validation.Email"]);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("必须包含大写字母")
            .Matches("[0-9]").WithMessage("必须包含数字");
    }
    
    // MudBlazor 集成：字段级验证
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => 
        async (model, propertyName) =>
        {
            var result = await ValidateAsync(
                ValidationContext<CreateUserDto>.CreateWithOptions(
                    (CreateUserDto)model,
                    x => x.IncludeProperties(propertyName)));
            
            return result.IsValid 
                ? Array.Empty<string>() 
                : result.Errors.Select(e => e.ErrorMessage);
        };
}
```

### 13.4 API 错误处理规范

```csharp
// 统一 API 响应结构
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    
    public static ApiResponse<T> Ok(T data) =>
        new() { Success = true, Code = 200, Data = data };
    
    public static ApiResponse<T> Fail(string message, int code = 400) =>
        new() { Success = false, Code = code, Message = message };
}

// 全局 HTTP 错误拦截
public class ApiExceptionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var response = await base.SendAsync(request, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content
                .ReadFromJsonAsync<ApiResponse<object>>(ct);
            
            throw response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => 
                    new UnauthorizedException(error?.Message),
                HttpStatusCode.Forbidden => 
                    new ForbiddenException(error?.Message),
                HttpStatusCode.NotFound => 
                    new NotFoundException(error?.Message),
                _ => new ApiException(error?.Message ?? "请求失败", 
                                      (int)response.StatusCode)
            };
        }
        
        return response;
    }
}
```

### 13.5 权限控制规范

```razor
<!-- 页面级权限 -->
@attribute [Authorize(Policy = "UserManagement.Read")]

<!-- 组件级权限显示 -->
<AuthorizeView Policy="UserManagement.Write">
    <Authorized>
        <MudButton OnClick="@OpenCreateDialog">
            @L["Common.Create"]
        </MudButton>
    </Authorized>
</AuthorizeView>

<!-- 按钮级权限 -->
<PermissionGuard Permission="user:delete">
    <MudIconButton Icon="@Icons.Material.Outlined.Delete"
                   OnClick="@(() => ConfirmDelete(item))" />
</PermissionGuard>
```

```csharp
// 权限策略注册
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserManagement.Read", 
        p => p.RequireClaim("permission", "user:read"));
    options.AddPolicy("UserManagement.Write", 
        p => p.RequireClaim("permission", "user:write"));
    options.AddPolicy("UserManagement.Delete", 
        p => p.RequireClaim("permission", "user:delete"));
});
```

---

## 14. 性能优化

### 14.1 组件优化

```razor
<!-- 使用 ShouldRender 控制重渲染 -->
@code {
    private string _previousValue = string.Empty;
    
    protected override bool ShouldRender()
    {
        if (_value == _previousValue) return false;
        _previousValue = _value;
        return true;
    }
}

<!-- 大列表使用虚拟化 -->
<MudVirtualize Items="@_largeList" ItemSize="48" Context="item">
    <MudListItem>@item.Name</MudListItem>
</MudVirtualize>

<!-- 懒加载页面组件 -->
@using Microsoft.AspNetCore.Components
<Suspense>
    <ChildContent>
        <LazyComponent />
    </ChildContent>
    <Fallback>
        <MudSkeleton Height="200px" />
    </Fallback>
</Suspense>
```

### 14.2 图片与资源优化

```html
<!-- 图片懒加载 -->
<img src="/images/avatar.webp" 
     loading="lazy" 
     decoding="async"
     width="48" height="48"
     alt="用户头像" />

<!-- 图标使用 SVG Sprite -->
<svg>
    <use href="/icons/sprite.svg#icon-name" />
</svg>
```

### 14.3 首屏优化（Blazor WASM）

```html
<!-- index.html：优化首屏加载体验 -->
<div id="app">
    <!-- 骨架屏（在 WASM 下载期间显示） -->
    <div class="skeleton-layout">
        <div class="skeleton-appbar"></div>
        <div class="skeleton-body">
            <div class="skeleton-sidebar"></div>
            <div class="skeleton-content"></div>
        </div>
    </div>
</div>
```

### 14.4 数据缓存策略

```csharp
// 菜单、字典等静态数据缓存
public class CachedMenuService : IMenuService
{
    private readonly IMemoryCache _cache;
    private const string CacheKey = "app:menus";
    
    public async Task<List<MenuDto>> GetMenusAsync()
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _innerService.GetMenusAsync();
        }) ?? new List<MenuDto>();
    }
}
```

---

## 15. 部署指南

### 15.1 Blazor Server 部署

```dockerfile
# Dockerfile.Server
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AdminPro.Server/AdminPro.Server.csproj", "AdminPro.Server/"]
COPY ["AdminPro.Shared/AdminPro.Shared.csproj", "AdminPro.Shared/"]
RUN dotnet restore "AdminPro.Server/AdminPro.Server.csproj"
COPY . .
RUN dotnet build "AdminPro.Server/AdminPro.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminPro.Server/AdminPro.Server.csproj" \
    -c Release -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminPro.Server.dll"]
```

### 15.2 Blazor WASM 部署（Nginx）

```nginx
# nginx.conf
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;
    
    # Brotli 压缩（优先）
    location ~ \.br$ {
        add_header Content-Encoding br;
        add_header Cache-Control "public, max-age=604800, immutable";
    }
    
    # Gzip 压缩
    location ~ \.gz$ {
        add_header Content-Encoding gzip;
        add_header Cache-Control "public, max-age=604800, immutable";
    }
    
    # SPA 路由回退（关键！）
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # 静态资源长缓存
    location /_framework/ {
        add_header Cache-Control "public, max-age=604800, immutable";
    }
    
    # API 代理
    location /api/ {
        proxy_pass http://api-service:8080/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### 15.3 环境配置

```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "#{DB_CONNECTION_STRING}#"
  },
  "Jwt": {
    "Secret": "#{JWT_SECRET}#",
    "Issuer": "AdminPro",
    "Audience": "AdminProClient",
    "ExpiryHours": 8
  },
  "Serilog": {
    "MinimumLevel": "Warning",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "Seq", "Args": { "serverUrl": "#{SEQ_URL}#" } }
    ]
  },
  "AllowedOrigins": ["https://admin.yourdomain.com"]
}
```

### 15.4 CI/CD 流水线（GitHub Actions）

```yaml
# .github/workflows/deploy.yml
name: Deploy Admin Pro

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore
        run: dotnet restore
      
      - name: Test
        run: dotnet test --no-restore --verbosity normal
      
      - name: Build Docker Image
        run: |
          docker build -f Dockerfile.Server \
            -t adminpro:${{ github.sha }} .
      
      - name: Deploy to Production
        run: |
          # 推送镜像并更新 K8s 部署
          kubectl set image deployment/adminpro \
            adminpro=adminpro:${{ github.sha }}
```

---

## 附录 A：设计检查清单

在提交 PR 之前，确保以下所有项目都已完成：

### 设计合规
- [ ] 颜色使用 Theme Token，不使用硬编码颜色
- [ ] 间距遵循 4px 网格系统
- [ ] 字体大小使用预定义字阶
- [ ] 图标风格统一（Outlined/Filled 按规范）
- [ ] 圆角统一（`rounded-lg` = 8px）

### 响应式
- [ ] 在 360px 宽度下无横向滚动
- [ ] 表格在移动端正确卡片化
- [ ] 对话框在移动端全屏显示
- [ ] 触摸目标最小 44x44px

### 国际化
- [ ] 所有文字使用 `@L["key"]` 而非硬编码
- [ ] 没有中文/英文硬编码在代码中
- [ ] 日期/数字格式根据语言适配

### 主题
- [ ] 暗色模式下所有元素可见
- [ ] 不使用 `color: black` 等绝对颜色
- [ ] 自定义 CSS 使用 `var(--mud-palette-*)` 变量

### 可访问性
- [ ] 图标按钮有 `aria-label` 或 `Tooltip`
- [ ] 表单字段有 `label`
- [ ] 颜色对比度满足 WCAG AA（≥4.5:1）
- [ ] 可键盘导航（Tab 顺序合理）

### 性能
- [ ] 大列表使用虚拟化
- [ ] 避免不必要的 `StateHasChanged()`
- [ ] 图片使用 `loading="lazy"`
- [ ] 异步操作有 Loading 状态

---

## 附录 B：常用代码片段

### B.1 删除确认对话框调用

```csharp
private async Task ConfirmDelete(UserDto user)
{
    var parameters = new DialogParameters
    {
        { nameof(ConfirmDeleteDialog.Message), L["Message.DeleteConfirm"].Value },
        { nameof(ConfirmDeleteDialog.ItemName), user.Name }
    };
    
    var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(
        L["Common.DeleteConfirmTitle"],
        parameters,
        new DialogOptions 
        { 
            MaxWidth = MaxWidth.ExtraSmall, 
            FullWidth = true 
        });
    
    var result = await dialog.Result;
    if (!result.Canceled)
    {
        await DeleteUserAsync(user.Id);
    }
}
```

### B.2 标准服务端数据加载

```csharp
private async Task<GridData<UserDto>> LoadServerData(GridState<UserDto> state)
{
    var query = new UserQueryDto
    {
        Page = state.Page + 1,
        PageSize = state.PageSize,
        SortField = state.SortDefinitions.FirstOrDefault()?.SortBy,
        SortDesc = state.SortDefinitions.FirstOrDefault()?.Descending ?? false,
        // ...搜索条件
    };
    
    var result = await UserService.GetUsersAsync(query);
    
    return new GridData<UserDto>
    {
        Items = result.Items,
        TotalItems = result.Total
    };
}
```

### B.3 文件导出

```csharp
private async Task ExportToExcel()
{
    _exporting = true;
    try
    {
        var data = await UserService.GetAllUsersAsync(_query);
        var bytes = ExcelHelper.Export(data);
        
        await JSRuntime.InvokeVoidAsync("downloadFile", 
            $"users_{DateTime.Now:yyyyMMdd}.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Convert.ToBase64String(bytes));
        
        Snackbar.Add(L["Message.ExportSuccess"], Severity.Success);
    }
    finally
    {
        _exporting = false;
    }
}
```

---

## 附录 C：版本历史

| 版本 | 日期 | 变更说明 |
|------|------|---------|
| v1.0.0 | 2025-01 | 初始版本，基于 MudBlazor 8.x + .NET 8 |

---

*文档维护：技术架构组 · 如有问题请提交 Issue 至内部 GitLab*
