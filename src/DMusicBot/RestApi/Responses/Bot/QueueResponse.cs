using Lavalink4NET.Players;

namespace DMusicBot.RestApi.Responses.Bot;

public struct QueueResponse
{
    public ITrackQueueItem[] Queue { get; init; }
}