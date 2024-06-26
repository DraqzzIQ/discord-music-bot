using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using DMusicBot.Util;

namespace DMusicBot.Modules;
public sealed class PositionModule(IAudioService audioService, ILogger<PositionModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Shows the track position asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
    public async Task Position()
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


        string response = $"Position: {TimeSpanFormatter.FormatDuration(player.Position?.Position)} / {TimeSpanFormatter.FormatDuration(player.CurrentTrack?.Duration)}.";

        await RespondAsync(response).ConfigureAwait(false);
    }
}