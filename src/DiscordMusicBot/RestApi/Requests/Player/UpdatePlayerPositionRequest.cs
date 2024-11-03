using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class UpdatePlayerPositionRequest : GuildRequest
{
    // The position in seconds
    [FromQuery] public int PositionInSeconds { get; init; }
}