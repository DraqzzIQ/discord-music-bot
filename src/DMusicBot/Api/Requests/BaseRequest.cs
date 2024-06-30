using Lavalink4NET;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.Api.Requests;

public class BaseRequest
{
    [FromHeader] public ulong GuildId { get; init; }
    [FromServices] public IAudioService AudioService { get; init; }
}