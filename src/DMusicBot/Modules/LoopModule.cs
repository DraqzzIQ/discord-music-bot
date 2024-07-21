using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Players.Queued;

namespace DMusicBot.Modules;
public sealed class LoopModule(IAudioService audioService, ILogger<LoopModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Loops the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("loop", description: "Loops the current track.", runMode: RunMode.Async)]
    public async Task LoopAsync()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        if (player.RepeatMode != TrackRepeatMode.Track)
        {
            player.RepeatMode = TrackRepeatMode.Track;
            await RespondAsync("Looping enabled.").ConfigureAwait(false);
            return;
        }

        player.RepeatMode = TrackRepeatMode.None;
        await RespondAsync("Looping disabled.").ConfigureAwait(false);
    }
}