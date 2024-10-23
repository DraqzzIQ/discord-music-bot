using System.Security.Claims;
using System.Text.Encodings.Web;
using DiscordMusicBot.Models;
using DiscordMusicBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DiscordMusicBot.RestApi.Auth;

public class CustomAuthenticationHandler(
    IDbService dbService,
    IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<CustomAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? tokenValue = null;

        // Attempt to get the token from the cookie
        if (Request.Cookies.TryGetValue("authToken", out string? cookieValue))
            tokenValue = cookieValue;

        // If not found in the cookie, try to retrieve the token from the Authorization header
        if (string.IsNullOrEmpty(tokenValue) && Request.Headers.TryGetValue(CustomAuthenticationSchemeOptions.AuthorizationHeaderName, out StringValues authHeader))
            tokenValue = authHeader;


        if (string.IsNullOrEmpty(tokenValue))
            return AuthenticateResult.Fail("Unauthorized");

        if (!Guid.TryParse(tokenValue, out Guid token))
            return AuthenticateResult.Fail("Invalid token");


        UserModel? auth = await dbService.GetUserAsync(token).ConfigureAwait(false);
        if (auth is null)
            return AuthenticateResult.Fail("Unauthorized");

        var claims = new List<Claim>
        {
            new("UserId", auth.Value.UserId.ToString()),
        };

        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);

        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}