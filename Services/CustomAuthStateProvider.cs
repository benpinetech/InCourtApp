using Microsoft.AspNetCore.Components.Authorization;

namespace InCourtApp.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _auth;

    public CustomAuthStateProvider(AuthService auth)
    {
        _auth = auth;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _auth.GetPrincipal();
        return Task.FromResult(new AuthenticationState(user));
    }

    public void NotifyAuthenticationStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
