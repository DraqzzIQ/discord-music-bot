using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class RemoveFromQueueRequest : GuildRequest
{
    [FromQuery] public int Index { get; init; }
}