using DMusicBot.Dtos;
using Lavalink4NET.Integrations.Lavasrc;

namespace DMusicBot.Extensions;

public static class ExtendedPlaylistInformationExtension
{
    public static PlaylistDto ToPlaylistDto(this ExtendedPlaylistInformation playlist)
    {
        return new PlaylistDto
        {
            Title = playlist.Name,
            Author = playlist.Author,
            Url = playlist.Uri?.ToString(),
            ArtworkUrl = playlist.ArtworkUri,
            TrackCount = playlist.TotalTracks,
            SelectedTrack = playlist.SelectedTrack?.Track.ToTrackDto()
        };
    }
}