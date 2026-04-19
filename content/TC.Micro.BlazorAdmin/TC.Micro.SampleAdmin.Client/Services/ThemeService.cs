using MudBlazor;

namespace TC.Micro.SampleAdmin.Client.Services;

/// <summary>
/// 主题服务实现（遵循 MudBlazor 企业级设计规范 §6.1）
/// 支持明/暗切换，颜色体系基于 Material Design 3。
/// </summary>
public class ThemeService : IThemeService
{
    public MudTheme CurrentTheme { get; } = CreateTheme();
    public bool IsDarkMode { get; private set; } = false;

    public event Action? OnThemeChanged;

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        OnThemeChanged?.Invoke();
    }

    public void SetDarkMode(bool isDark)
    {
        if (IsDarkMode == isDark) return;
        IsDarkMode = isDark;
        OnThemeChanged?.Invoke();
    }

    private static MudTheme CreateTheme() => new()
    {
        PaletteLight = new PaletteLight
        {
            // ── 主色（Material Blue 700）──────────────────
            Primary             = "#1976D2",
            PrimaryContrastText = "#FFFFFF",
            Secondary           = "#00BCD4",
            SecondaryContrastText = "#FFFFFF",
            Tertiary            = "#FF6F00",

            // ── 功能色 ────────────────────────────────────
            Success = "#4CAF50",
            Warning = "#FF9800",
            Error   = "#F44336",
            Info    = "#2196F3",

            // ── 背景 ──────────────────────────────────────
            Background     = "#F5F7FA",
            Surface        = "#FFFFFF",
            BackgroundGray = "#F0F2F5",

            // ── AppBar / Drawer ───────────────────────────
            AppbarBackground = "#FFFFFF",
            AppbarText       = "rgba(0,0,0,0.87)",
            DrawerBackground = "#FFFFFF",
            DrawerText       = "rgba(0,0,0,0.87)",
            DrawerIcon       = "rgba(0,0,0,0.54)",

            // ── 文字 ──────────────────────────────────────
            TextPrimary   = "rgba(0,0,0,0.87)",
            TextSecondary = "rgba(0,0,0,0.60)",
            TextDisabled  = "rgba(0,0,0,0.38)",

            // ── 分隔线 ────────────────────────────────────
            Divider       = "rgba(0,0,0,0.12)",
            DividerLight  = "rgba(0,0,0,0.06)",
            TableLines    = "rgba(0,0,0,0.08)",
            LinesDefault  = "rgba(0,0,0,0.12)"
        },

        PaletteDark = new PaletteDark
        {
            // ── 主色（暗色模式适当调亮）──────────────────
            Primary             = "#42A5F5",
            PrimaryContrastText = "#000000",
            Secondary           = "#26C6DA",
            SecondaryContrastText = "#000000",
            Tertiary            = "#FFA726",

            // ── 功能色 ────────────────────────────────────
            Success = "#66BB6A",
            Warning = "#FFA726",
            Error   = "#EF5350",
            Info    = "#42A5F5",

            // ── 背景 ──────────────────────────────────────
            Background     = "#111827",
            Surface        = "#1F2937",
            BackgroundGray = "#374151",

            // ── AppBar / Drawer ───────────────────────────
            AppbarBackground = "#1F2937",
            AppbarText       = "rgba(255,255,255,0.87)",
            DrawerBackground = "#1F2937",
            DrawerText       = "rgba(255,255,255,0.87)",
            DrawerIcon       = "rgba(255,255,255,0.54)",

            // ── 文字 ──────────────────────────────────────
            TextPrimary   = "rgba(255,255,255,0.87)",
            TextSecondary = "rgba(255,255,255,0.60)",
            TextDisabled  = "rgba(255,255,255,0.38)",

            // ── 分隔线 ────────────────────────────────────
            Divider      = "rgba(255,255,255,0.12)",
            DividerLight = "rgba(255,255,255,0.06)",
            TableLines   = "rgba(255,255,255,0.08)",
            LinesDefault = "rgba(255,255,255,0.12)"
        },

        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = new[] { "Roboto", "Noto Sans SC", "sans-serif" },
                FontSize   = "0.875rem",
                LineHeight = "1.5"
            },
            H4 = new H4 { FontSize = "2.125rem", FontWeight = "400" },
            H5 = new H5 { FontSize = "1.5rem",   FontWeight = "400" },
            H6 = new H6 { FontSize = "1.25rem",  FontWeight = "500" }
        }
    };
}
