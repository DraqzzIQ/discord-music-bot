using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class ReorderQueueRequest : GuildRequest
{
    [FromQuery] public int SourceIndex { get; init; }
    [FromQuery] public int DestinationIndex { get; init; }
}