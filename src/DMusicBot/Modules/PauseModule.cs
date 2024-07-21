using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Players;

namespace DMusicBot.Modules;
public sealed class PauseModule(IAudioService audioService, ILogger<PauseModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Pauses the music asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
    public async Task PauseAsync()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        if (player.State is PlayerState.Paused)
        {
            await RespondAsync("Player is already paused.").ConfigureAwait(false);
            return;
        }

        await player.PauseAsync().ConfigureAwait(false);
        await RespondAsync("Paused.").ConfigureAwait(false);
    }
}