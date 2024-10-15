using DiscordMusicBot.Dtos;
using Lavalink4NET.Players;
using Lavalink4NET.Tracks;

namespace DiscordMusicBot.Extensions;

public static class TrackQueueItemExtension
{
    public static TrackDto ToTrackDto(this ITrackQueueItem item)
    {
        LavalinkTrack? track = item.Track;
        if (track is null)
            return new TrackDto();
        
        return new TrackDto
        {
            Title = track.Title,
            Author = track.Author,
            DurationInSeconds = (int) track.Duration.TotalSeconds,
            ThumbnailUrl = track.ArtworkUri,
            Url = track.Uri
        };
    }
}