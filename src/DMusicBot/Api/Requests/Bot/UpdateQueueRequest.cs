using Lavalink4NET.Players;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.Api.Requests.Bot;

public class UpdateQueueRequest : BaseRequest
{
    [FromBody] public ITrackQueueItem[] Queue { get; init; }
}