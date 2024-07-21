using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdateRepeatModeRequest : BaseRequest
{
    [FromBody] public TrackRepeatMode RepeatMode { get; init; }
}