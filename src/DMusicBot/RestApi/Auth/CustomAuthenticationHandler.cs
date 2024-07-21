using System.Security.Claims;
using System.Text.Encodings.Web;
using DMusicBot.Models;
using DMusicBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DMusicBot.RestApi.Auth;

public class CustomAuthenticationHandler(
    IDbService dbService,
    IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<CustomAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IDbService _dbService = dbService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? tokenValue = null;

        // Attempt to get the token from the cookie
        if (Request.Cookies.TryGetValue("authCookie", out string? cookieValue))
            tokenValue = cookieValue;

        // If not found in the cookie, try to retrieve the token from the Authorization header
        if (string.IsNullOrEmpty(tokenValue) && Request.Headers.TryGetValue(CustomAuthenticationSchemeOptions.AuthorizationHeaderName, out StringValues authHeader))
            tokenValue = authHeader;


        if (string.IsNullOrEmpty(tokenValue))
            return AuthenticateResult.Fail("Unauthorized");

        if (!Guid.TryParse(tokenValue, out Guid token))
            return AuthenticateResult.Fail("Invalid token");


        AuthModel? auth = await _dbService.GetAuthTokenAsync(token);
        if (auth is null)
            return AuthenticateResult.Fail("Unauthorized");

        var claims = new List<Claim>
        {
            new("UserId", auth.Value.UserId.ToString()),
            new("GuildId", auth.Value.GuildId.ToString())
        };

        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);

        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}