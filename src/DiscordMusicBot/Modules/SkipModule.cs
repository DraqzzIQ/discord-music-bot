using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class SkipModule(
    IAudioService audioService,
    ILogger<SkipModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Skips the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("skip", "Skips the current track", runMode: RunMode.Async)]
    public async Task Skip([Summary("count", "How many tracks to skip")] [MinValue(1)] int count = 1)
    {
        var player = await GetPlayerAsync(false).ConfigureAwait(false);

        if (player is null) return;

        if (player.CurrentItem is null)
        {
            await RespondAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        await player.SkipSignalRAsync(count).ConfigureAwait(false);

        var track = player.CurrentItem;

        if (track is not null)
            await RespondAsync("Skipped.").ConfigureAwait(false);
        else
            await RespondAsync("Skipped. Stopped playing because the queue is now empty.").ConfigureAwait(false);
    }
}