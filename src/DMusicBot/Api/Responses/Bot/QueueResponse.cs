using Lavalink4NET.Players;

namespace DMusicBot.Api.Responses.Bot;

public struct QueueResponse
{
    public ITrackQueueItem[] Queue { get; init; }
}