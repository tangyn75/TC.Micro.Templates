using System.Net.Http.Headers;

namespace TC.Micro.SampleAdmin.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly WasmAuthStateProvider _authProvider;

    public AuthMessageHandler(WasmAuthStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authProvider.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
