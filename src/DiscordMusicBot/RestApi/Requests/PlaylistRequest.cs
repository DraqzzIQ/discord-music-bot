using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests;

public class PlaylistRequest : GuildRequest
{
    [FromQuery] public string PlaylistId { get; init; }
}