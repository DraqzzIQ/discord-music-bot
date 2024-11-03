using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class GetAddPlaylistToPlaylistPreviewRequest : GuildRequest
{
    [FromQuery] public string? PlaylistUrl { get; init; }
    [FromQuery] public string? EncodedTracks { get; init; }

    public string[]? ParsedEncodedTracks => EncodedTracks?.Split(',');
}