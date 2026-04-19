using MudBlazor;

namespace TC.Micro.Sample.Admin.Client.Services;

/// <summary>
/// 主题服务实现（遵循 MudBlazor 企业级设计规范 §6.1）
/// Material Design 3 色彩体系，支持明/暗切换。
/// </summary>
public class ThemeService : IThemeService
{
    public MudTheme CurrentTheme { get; } = CreateTheme();
    public bool IsDarkMode { get; private set; }

    public event Action? OnThemeChanged;

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        OnThemeChanged?.Invoke();
    }

    private static MudTheme CreateTheme() => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary             = "#1976D2",
            PrimaryContrastText = "#FFFFFF",
            Secondary           = "#00BCD4",
            Success             = "#4CAF50",
            Warning             = "#FF9800",
            Error               = "#F44336",
            Info                = "#2196F3",
            Background          = "#F5F7FA",
            Surface             = "#FFFFFF",
            AppbarBackground    = "#FFFFFF",
            AppbarText          = "rgba(0,0,0,0.87)",
            DrawerBackground    = "#FFFFFF",
            DrawerText          = "rgba(0,0,0,0.87)",
            TextPrimary         = "rgba(0,0,0,0.87)",
            TextSecondary       = "rgba(0,0,0,0.60)",
            Divider             = "rgba(0,0,0,0.12)"
        },
        PaletteDark = new PaletteDark
        {
            Primary             = "#42A5F5",
            PrimaryContrastText = "#000000",
            Secondary           = "#26C6DA",
            Success             = "#66BB6A",
            Warning             = "#FFA726",
            Error               = "#EF5350",
            Info                = "#42A5F5",
            Background          = "#111827",
            Surface             = "#1F2937",
            AppbarBackground    = "#1F2937",
            AppbarText          = "rgba(255,255,255,0.87)",
            DrawerBackground    = "#1F2937",
            DrawerText          = "rgba(255,255,255,0.87)",
            TextPrimary         = "rgba(255,255,255,0.87)",
            TextSecondary       = "rgba(255,255,255,0.60)",
            Divider             = "rgba(255,255,255,0.12)"
        }
    };
}
