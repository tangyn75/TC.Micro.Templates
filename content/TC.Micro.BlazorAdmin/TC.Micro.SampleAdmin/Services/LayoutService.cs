namespace TC.Micro.SampleAdmin.Services;

public class LayoutService : ILayoutService
{
    private readonly IWebHostEnvironment _env;

    public LayoutService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool IsDevelopment => _env.IsDevelopment();
}
