using Lavalink4NET.Players;

namespace DiscordMusicBot.RestApi.Responses.Bot;

public struct QueueResponse
{
    public ITrackQueueItem[] Queue { get; init; }
}