using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdatePlayerPositionRequest : BaseRequest
{
    // The position in seconds
    [FromBody] public int PositionInSeconds { get; init; }
}