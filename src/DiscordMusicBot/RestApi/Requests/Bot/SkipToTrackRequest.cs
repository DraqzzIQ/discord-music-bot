using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Bot;

public class SkipToTrackRequest : GuildRequest
{
    [FromQuery] public int Index { get; init; }
}