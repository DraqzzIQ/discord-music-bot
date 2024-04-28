using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using DMusicBot.Util;

namespace DMusicBot.Modules;
public sealed class SeekModule(IAudioService audioService, ILogger<SeekModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Seeks to the provided position asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("seek", description: "Seeks to the provided position", runMode: RunMode.Async)]
    public async Task Seek([MinValue(0)] uint seconds)
    {
        TimeSpan position = TimeSpan.FromSeconds(seconds);

        var player = await GetPlayerAsync(connectToVoiceChannel: false);

        if (player is null)
        {
            return;
        }

        if (player.CurrentItem is null)
        {
            await RespondAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        await player.SeekAsync(position).ConfigureAwait(false);


        await RespondAsync($"Seeked to {TimeSpanFormatter.FormatDuration(position)}.").ConfigureAwait(false);
    }
}