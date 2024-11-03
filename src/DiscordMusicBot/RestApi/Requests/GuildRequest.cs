using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests;

public class GuildRequest : BaseRequest
{
    [FromQuery] public ulong GuildId { get; init; }
}