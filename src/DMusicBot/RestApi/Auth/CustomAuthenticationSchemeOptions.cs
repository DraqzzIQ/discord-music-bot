using Microsoft.AspNetCore.Authentication;

namespace DMusicBot.RestApi.Auth;

public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "CustomAuth";
    public const string AuthorizationHeaderName = "Authorization";
}