using DiscordMusicBot.Dtos;
using Lavalink4NET.Tracks;

namespace DiscordMusicBot.Extensions;

public static class LavalinkTrackExtension
{
    public static TrackDto ToTrackDto(this LavalinkTrack track, bool encode = false)
    {
        return new TrackDto
        {
            Title = track.Title,
            Author = track.Author,
            DurationInSeconds = (int)track.Duration.TotalSeconds,
            ArtworkUrl = track.ArtworkUri,
            Url = track.Uri,
            EncodedTrack = encode ? track.ToString() : null
        };
    }
}