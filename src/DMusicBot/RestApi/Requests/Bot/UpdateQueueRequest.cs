using DMusicBot.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdateQueueRequest : GuildRequest
{
    [FromBody] public TrackDto[] Queue { get; init; }
}