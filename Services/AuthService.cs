using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace InCourtApp.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;

    public string? Token { get; private set; }

    public AuthService(HttpClient http, ILogger<AuthService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("api/login", new { username, password });
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("token", out var tokenElement))
                {
                    Token = tokenElement.GetString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse login response");
            }
        }
        return false;
    }

    public void Logout()
    {
        Token = null;
    }

    public ClaimsPrincipal GetPrincipal()
    {
        if (string.IsNullOrEmpty(Token))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(Token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }
}
