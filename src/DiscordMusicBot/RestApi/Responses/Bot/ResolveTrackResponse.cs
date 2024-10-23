using Lavalink4NET.Tracks;

namespace DiscordMusicBot.RestApi.Responses.Bot;

public class ResolveTrackResponse
{
    public LavalinkTrack Track { get; init; }
}