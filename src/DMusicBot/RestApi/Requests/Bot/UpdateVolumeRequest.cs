using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class UpdateVolumeRequest : BaseRequest
{
    [FromBody] public float Volume { get; init; }
}