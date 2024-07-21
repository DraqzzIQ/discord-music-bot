using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Players.Queued;

namespace DMusicBot.Modules;
public sealed class LoopQueueModule(IAudioService audioService, ILogger<LoopQueueModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Loops the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("loop_queue", description: "Loops the queue.", runMode: RunMode.Async)]
    public async Task LoopQueueAsync()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

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