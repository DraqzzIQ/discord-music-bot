using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.RestApi.Requests.Player;

public class PlayPlaylistRequest : PlaylistRequest
{
    [FromQuery] public bool ShouldPlay { get; init; }
    [FromServices] public IHubContext<BotHub, IBotClient> HubContext { get; init; }
}