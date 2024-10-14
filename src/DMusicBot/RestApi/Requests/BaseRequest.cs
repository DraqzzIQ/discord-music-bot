using System.Security.Claims;
using Discord.WebSocket;
using DMusicBot.Services;
using Lavalink4NET;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests;

public class BaseRequest
{
    [FromServices] public IAudioService AudioService { get; init; }
    [FromServices] public IDbService DbService { get; init; }
    [FromServices] public DiscordSocketClient DiscordSocketClient { get; init; }
    
    public ClaimsPrincipal User { get; init; }
    public ulong UserId => ulong.Parse(User.FindFirst("UserId")?.Value ?? "0");
}