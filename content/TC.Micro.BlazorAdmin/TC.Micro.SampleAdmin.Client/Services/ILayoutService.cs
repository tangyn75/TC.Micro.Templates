namespace TC.Micro.SampleAdmin.Client.Services;

/// <summary>布局服务接口（Drawer 状态、环境判断等）</summary>
public interface ILayoutService
{
    bool IsDevelopment { get; }
}
