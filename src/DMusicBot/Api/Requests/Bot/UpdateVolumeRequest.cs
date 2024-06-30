using Lavalink4NET;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.Api.Requests.Bot;

public class UpdateVolumeRequest : BaseRequest
{
    [FromBody] public float Volume { get; init; }
}