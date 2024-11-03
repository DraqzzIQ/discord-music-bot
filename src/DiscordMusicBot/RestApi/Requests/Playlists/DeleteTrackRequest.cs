using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class DeleteTrackRequest : PlaylistRequest
{
    [FromQuery] public string? TrackId { get; init; }
    [FromQuery] public Uri? TrackUrl { get; init; }
}