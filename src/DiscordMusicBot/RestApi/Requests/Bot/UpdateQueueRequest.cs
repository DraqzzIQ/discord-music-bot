using DiscordMusicBot.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Bot;

public class UpdateQueueRequest : GuildRequest
{
    [FromBody] public TrackDto[] Queue { get; init; }
}