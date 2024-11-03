using DiscordMusicBot.Audio;
using DiscordMusicBot.Dtos;
using DiscordMusicBot.Extensions;
using DiscordMusicBot.Services;
using DiscordMusicBot.SignalR.Clients;
using Lavalink4NET;
using Lavalink4NET.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.SignalR.Hubs;

[Authorize]
public sealed class BotHub : Hub<IBotClient>
{
    private static readonly Dictionary<string, ulong> Connections = new();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Connections.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToPlayer(string guildId, IDbService dbService)
    {
        var valid = ulong.TryParse(guildId, out var id);
        if (!valid)
            return;

        var userId = ulong.Parse(Context.User?.FindFirst("UserId")?.Value ?? "0");
        var user = await dbService.GetUserAsync(userId).ConfigureAwait(false);
        if (!user.HasValue || !user.Value.GuildIds.Contains(id))
            return;

        Connections[Context.ConnectionId] = id;
        await Groups.AddToGroupAsync(Context.ConnectionId, guildId);
    }

    public async Task GetPlayerStatus(IAudioService audioService)
    {
        if (!Connections.TryGetValue(Context.ConnectionId, out var guildId))
            return;

        var player = (SignalRPlayer?)audioService.Players.Players.FirstOrDefault(x => x.GuildId == guildId);
        if (player is null)
        {
            await Clients.Caller.UpdatePlayer(new PlayerUpdatedDto
            {
                UpdateQueue = false,
                CurrentTrack = null,
                PositionInSeconds = 0,
                Queue = [],
                State = PlayerState.Destroyed
            });
            return;
        }

        await Clients.Caller.UpdatePlayer(new PlayerUpdatedDto
        {
            UpdateQueue = true,
            CurrentTrack = player.CurrentItem?.ToTrackDto(),
            PositionInSeconds = player.Position.HasValue ? (int)player.Position.Value.Position.TotalSeconds : 0,
            Queue = player.Queue.Select(x => x.ToTrackDto()).ToArray(),
            State = player.State
        });
    }
}