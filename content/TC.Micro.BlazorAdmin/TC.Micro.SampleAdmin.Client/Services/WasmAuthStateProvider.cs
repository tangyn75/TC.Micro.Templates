using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TC.Micro.SampleAdmin.Client.Services;

public class WasmAuthStateProvider : AuthenticationStateProvider
{
    private string? _token;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(_token))
            return Task.FromResult(new AuthenticationState(_anonymous));

        var identity = ParseClaimsFromJwt(_token);
        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    public void SetToken(string? token)
    {
        _token = token;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public string? GetToken() => _token;

    private static ClaimsIdentity ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        var claims = token.Claims.ToList();

        return new ClaimsIdentity(claims, "jwt");
    }
}
