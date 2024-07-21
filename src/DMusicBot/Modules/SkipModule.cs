using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;
public sealed class SkipModule(IAudioService audioService, ILogger<SkipModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Skips the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
    public async Task Skip([Summary("count", "How many tracks to skip")][MinValue(1)] int count = 1)
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentItem is null)
        {
            await RespondAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        await player.SkipAsync(count).ConfigureAwait(false);

        var track = player.CurrentItem;

        if (track is not null)
        {
            await RespondAsync($"Skipped.").ConfigureAwait(false);
        }
        else
        {
            await RespondAsync("Skipped. Stopped playing because the queue is now empty.").ConfigureAwait(false);
        }
    }
}