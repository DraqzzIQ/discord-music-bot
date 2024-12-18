using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class SearchRequest : GuildRequest
{
    [FromQuery] public string Query { get; init; }
    [FromQuery] public string SearchMode { get; init; }
}