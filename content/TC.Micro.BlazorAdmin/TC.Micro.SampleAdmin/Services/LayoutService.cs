namespace TC.Micro.SampleAdmin.Services;

public class LayoutService : ILayoutService
{
#if (IsServer || IsAuto)
    private readonly IWebHostEnvironment _env;

    public LayoutService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool IsDevelopment => _env.IsDevelopment();
#endif
#if (IsWasm)
    private readonly IWebAssemblyHostEnvironment _env;

    public LayoutService(IWebAssemblyHostEnvironment env)
    {
        _env = env;
    }

    public bool IsDevelopment => _env.IsDevelopment();
#endif
}
