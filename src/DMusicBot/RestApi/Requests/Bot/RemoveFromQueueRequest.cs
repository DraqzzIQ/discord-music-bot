using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class RemoveFromQueueRequest : GuildRequest
{
    [FromQuery] public int Index { get; init; }
}