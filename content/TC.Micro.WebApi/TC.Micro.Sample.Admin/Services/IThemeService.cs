using MudBlazor;

namespace TC.Micro.Sample.Admin.Services;

/// <summary>主题服务接口（明/暗切换）</summary>
public interface IThemeService
{
    MudTheme CurrentTheme { get; }
    bool IsDarkMode { get; }
    event Action? OnThemeChanged;
    void ToggleDarkMode();
}
