using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class ReorderPlaylistRequest : PlaylistRequest
{
    [FromQuery] public int SourceIndex { get; init; }
    [FromQuery] public int DestinationIndex { get; init; }
}