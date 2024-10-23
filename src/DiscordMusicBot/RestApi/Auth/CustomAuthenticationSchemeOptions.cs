using Microsoft.AspNetCore.Authentication;

namespace DiscordMusicBot.RestApi.Auth;

public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "CustomAuth";
    public const string AuthorizationHeaderName = "Authorization";
}