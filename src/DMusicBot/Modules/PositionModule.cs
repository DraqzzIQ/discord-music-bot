using Discord.Interactions;
using DMusicBot.SignalR.Clients;
using DMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using DMusicBot.Util;
using Microsoft.AspNetCore.SignalR;

namespace DMusicBot.Modules;
public sealed class PositionModule(IAudioService audioService, ILogger<PositionModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
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