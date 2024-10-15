using Lavalink4NET.Rest.Entities.Tracks;

namespace DiscordMusicBot.Util;

public class TrackSearchModeParser
{
    public static TrackSearchMode Parse(string source)
    {
        return source switch
        {
            "Spotify" => TrackSearchMode.Spotify,
            "YouTube" => TrackSearchMode.YouTube,
            "YouTubeMusic" => TrackSearchMode.YouTubeMusic,
            "Deezer" => TrackSearchMode.Deezer,
            "Link" => TrackSearchMode.None,
            _ => TrackSearchMode.Deezer
        };
    }
}