using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class AddPlaylistToPlaylistRequest : PlaylistRequest
{
    [FromQuery] public string? PlaylistUrl { get; init; }
    [FromBody] public string[]? EncodedTracks { get; init; }
}