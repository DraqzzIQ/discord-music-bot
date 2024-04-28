using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Discord;
using DMusicBot.Util;
using System.Text;

namespace DMusicBot.Modules;
public sealed class QueueModule(IAudioService audioService, ILogger<QueueModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Displays the queue asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("queue", description: "Displays the queue", runMode: RunMode.Async)]
    public async Task Queue()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        // nothing playing
        if (player.CurrentItem is null)
        {
            await RespondAsync("Nothing playing!").ConfigureAwait(false);
            return;
        }

        TimeSpan totalDuration = TimeSpan.Zero;

        foreach (var track in player.Queue)
        {
            totalDuration += track.Track!.Duration;
        }


        var listBuilder = new StringBuilder();

        listBuilder.Append($"Now Playing: [{player.CurrentTrack!.Title}]({player.CurrentTrack!.Uri})\n");

        bool alreadyResponded = false;
        uint index = 2;

        foreach (var track in player.Queue)
        {
            if (listBuilder.Length + $"{index} [{track.Track!.Title}]({track.Track!.Uri})\n".Length > 4000)
            {
                Embed embed = EmbedCreator.CreateEmbed($"Queue (Total Duration: {TimeSpanFormatter.FormatDuration(totalDuration)}, Total Songs: {player.Queue.Count})", $"{listBuilder}", Color.Blue);
                if (!alreadyResponded)
                {
                    alreadyResponded = true;
                    await RespondAsync(embed: embed).ConfigureAwait(false);
                }
                else
                {
                    await FollowupAsync(embed: embed).ConfigureAwait(false);
                }
                listBuilder.Clear();
            }

            listBuilder.Append($"{index} [{track.Track!.Title}]({track.Track!.Uri})\n");
            index++;
        }

        Embed embed1 = EmbedCreator.CreateEmbed($"Queue (Total Duration: {TimeSpanFormatter.FormatDuration(totalDuration)}, Total Songs: {player.Queue.Count})", $"{listBuilder}", Color.Blue);

        if (!alreadyResponded)
            await RespondAsync(embed: embed1).ConfigureAwait(false);
        else
            await FollowupAsync(embed: embed1).ConfigureAwait(false);
    }
}