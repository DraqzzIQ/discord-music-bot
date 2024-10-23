using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.Audio;

public record SignalRPlayerOptions : QueuedLavalinkPlayerOptions
{
    public IHubContext<BotHub, IBotClient> HubContext { get; init; }
}