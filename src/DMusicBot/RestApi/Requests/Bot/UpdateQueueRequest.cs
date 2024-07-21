using Lavalink4NET.Players;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdateQueueRequest : BaseRequest
{
    [FromBody] public ITrackQueueItem[] Queue { get; init; }
}