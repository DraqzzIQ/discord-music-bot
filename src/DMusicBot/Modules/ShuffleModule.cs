using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;
public sealed class ShuffleModule(IAudioService audioService, ILogger<ShuffleModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Shuffles the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("shuffle", description: "Shuffles the queue", runMode: RunMode.Async)]
    public async Task Shuffle()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        await player.Queue.ShuffleAsync();

        await RespondAsync($"Queue shuffled").ConfigureAwait(false);
    }
}