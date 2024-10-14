using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdatePlayerPositionRequest : GuildRequest
{
    // The position in seconds
    [FromQuery] public int PositionInSeconds { get; init; }
}