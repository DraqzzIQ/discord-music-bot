using DiscordMusicBot.Dtos;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.RestApi.Requests.Bot;

public class PlayRequest : GuildRequest
{
    [FromBody] public PlayRequestDto PlayRequestDto { get; init; }
    [FromServices] public IHubContext<BotHub, IBotClient> HubContext { get; init; }
}