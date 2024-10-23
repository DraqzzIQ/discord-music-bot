using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracks;

namespace DiscordMusicBot.RestApi.Responses.Bot;

public struct StatusResponse
{
    public PlayerState State { get; init; }
    public TimeSpan? Position { get; init; }
    public float Volume { get; init; }
    public TrackRepeatMode RepeatMode { get; init; }
    public LavalinkTrack? Track { get; init; }
}