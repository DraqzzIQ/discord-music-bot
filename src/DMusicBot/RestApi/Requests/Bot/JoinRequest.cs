using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class JoinRequest : BaseRequest
{
    [FromServices] public DiscordSocketClient DiscordSocketClient { get; set; }
}