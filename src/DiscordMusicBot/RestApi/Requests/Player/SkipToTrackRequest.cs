using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class SkipToTrackRequest : GuildRequest
{
    [FromQuery] public int Index { get; init; }
}