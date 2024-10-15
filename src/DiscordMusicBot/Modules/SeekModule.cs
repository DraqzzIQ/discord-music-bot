using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace DiscordMusicBot.Modules;
public sealed class SeekModule(IAudioService audioService, ILogger<SeekModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Seeks to the provided position asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("seek", description: "Seeks to the provided position", runMode: RunMode.Async)]
    public async Task Seek([Summary("seconds", "The position in seconds to seek to")][MinValue(0)] uint seconds)
    {
        TimeSpan position = TimeSpan.FromSeconds(seconds);

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

        await player.SeekSignalRAsync(position).ConfigureAwait(false);


        await RespondAsync($"Seeked to {TimeSpanFormatter.FormatDuration(position)}.").ConfigureAwait(false);
    }
}