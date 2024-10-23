using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Bot;

public class RemoveFromQueueRequest : GuildRequest
{
    [FromQuery] public int Index { get; init; }
}