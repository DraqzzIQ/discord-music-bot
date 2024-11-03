using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class GetAddTrackToPlaylistPreviewRequest : GuildRequest
{
    [FromQuery] public Uri TrackUrl { get; set; }
}