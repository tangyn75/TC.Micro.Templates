namespace TC.Micro.SampleAdmin.Client.Services;

public class LayoutService : ILayoutService
{
    private readonly IWebAssemblyHostEnvironment _env;

    public LayoutService(IWebAssemblyHostEnvironment env)
    {
        _env = env;
    }

    public bool IsDevelopment => _env.IsDevelopment();
}
