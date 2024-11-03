using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class LoopQueueModule(
    IAudioService audioService,
    ILogger<LoopQueueModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Loops the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("loop_queue", "Loops the queue.", runMode: RunMode.Async)]
    public async Task LoopQueueAsync()
    {
        var player = await GetPlayerAsync(false).ConfigureAwait(false);

        if (player is null) return;

        if (player.RepeatMode != TrackRepeatMode.Queue)
        {
            player.RepeatMode = TrackRepeatMode.Queue;
            await RespondAsync("Looping queue enabled.").ConfigureAwait(false);
            return;
        }

        player.RepeatMode = TrackRepeatMode.None;
        await RespondAsync("Looping queue disabled.").ConfigureAwait(false);
    }
}