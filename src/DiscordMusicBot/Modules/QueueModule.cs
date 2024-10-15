using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Discord;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.Modules;

public sealed class QueueModule(IAudioService audioService, ILogger<QueueModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Displays the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("queue", description: "Displays the queue", runMode: RunMode.Async)]
    public async Task Queue()
    {
        await DeferAsync().ConfigureAwait(false);
        
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        // nothing playing
        if (player.CurrentItem is null)
        {
            await FollowupAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        TimeSpan totalDuration = player.Queue.Aggregate(TimeSpan.Zero, (current, track) => current + track.Track!.Duration);

        int index = 0;
        List<string> queueList = [$"Playing Now: [{player.CurrentTrack!.Title}]({player.CurrentTrack!.Uri})"];
        queueList.AddRange(player.Queue.Select(track => $"{++index + 1} [{track.Track!.Title}]({track.Track!.Uri})"));

        Embed[] embeds =
            EmbedCreator.CreateEmbeds($"Queue (Total Duration: {TimeSpanFormatter.FormatDuration(totalDuration)}, Total Tracks: {player.Queue.Count})",
                queueList);

        await FollowupAsync(embeds: embeds).ConfigureAwait(false);
    }
}