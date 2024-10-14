using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class JoinRequest : GuildRequest
{
    [FromServices] public DiscordSocketClient DiscordSocketClient { get; init; }
}