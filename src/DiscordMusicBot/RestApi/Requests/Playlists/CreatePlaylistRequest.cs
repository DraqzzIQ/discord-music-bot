using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class CreatePlaylistRequest : GuildRequest
{
    [FromQuery] public string Name { get; set; }
    [FromQuery] public bool IsPublic { get; set; }
}