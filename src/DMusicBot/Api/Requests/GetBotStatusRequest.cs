using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.Api.Requests;

internal record struct GetBotStatusRequest(
    [FromRoute] ulong GuildId,
    [FromRoute] ulong UserId);