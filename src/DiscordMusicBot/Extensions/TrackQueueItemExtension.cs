using DiscordMusicBot.Dtos;
using Lavalink4NET.Players;

namespace DiscordMusicBot.Extensions;

public static class TrackQueueItemExtension
{
    public static TrackDto ToTrackDto(this ITrackQueueItem item, bool encode = false)
    {
        var track = item.Track;
        if (track is null)
            return new TrackDto();

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