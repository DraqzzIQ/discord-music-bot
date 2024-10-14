using DMusicBot.SignalR.Clients;
using DMusicBot.SignalR.Hubs;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.SignalR;

namespace DMusicBot.Audio;

public record SignalRPlayerOptions : QueuedLavalinkPlayerOptions
{
    public IHubContext<BotHub, IBotClient> HubContext { get; init; }
}