using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdateRepeatModeRequest : GuildRequest
{
    [FromBody] public TrackRepeatMode RepeatMode { get; init; }
}