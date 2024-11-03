using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class ShuffleModule(
    IAudioService audioService,
    ILogger<ShuffleModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Shuffles the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("shuffle", "Shuffles the queue", runMode: RunMode.Async)]
    public async Task Shuffle()
    {
        var player = await GetPlayerAsync(false).ConfigureAwait(false);

        if (player is null) return;

        await player.ShuffleQueueSignalRAsync().ConfigureAwait(false);

        await RespondAsync("Queue shuffled").ConfigureAwait(false);
    }
}