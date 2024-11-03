using DiscordMusicBot.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class UpdateQueueRequest : GuildRequest
{
    [FromBody] public TrackDto[] Queue { get; init; }
}