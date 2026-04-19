using MudBlazor;

namespace TC.Micro.SampleAdmin.Client.Services;

/// <summary>主题服务接口（明/暗切换 + 系统跟随）</summary>
public interface IThemeService
{
    MudTheme CurrentTheme { get; }
    bool IsDarkMode { get; }
    event Action? OnThemeChanged;
    void ToggleDarkMode();
    void SetDarkMode(bool isDark);
}
