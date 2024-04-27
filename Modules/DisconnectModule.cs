using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;
public sealed class DisconnectModule(IAudioService audioService, ILogger<DisconnectModule> logger) : MusicModule(audioService, logger)
{
    /// <summary>
    ///     Disconnects from the current voice channel connected to asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
    public async Task Disconnect()
    {
        var player = await GetPlayerAsync().ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        await player.DisconnectAsync().ConfigureAwait(false);
        await RespondAsync("Disconnected.").ConfigureAwait(false);
    }
}