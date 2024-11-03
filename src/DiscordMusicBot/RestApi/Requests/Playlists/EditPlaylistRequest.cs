using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class EditPlaylistRequest : PlaylistRequest
{
    [FromQuery] public string? Name { get; init; }
    [FromQuery] public bool? IsPublic { get; init; }
}