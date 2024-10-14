using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class GuildRequest : BaseRequest
{
    [FromQuery] public ulong GuildId { get; init; }
}