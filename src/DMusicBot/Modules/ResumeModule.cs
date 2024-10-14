using Discord.Interactions;
using DMusicBot.SignalR.Clients;
using DMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Players;
using Microsoft.AspNetCore.SignalR;

namespace DMusicBot.Modules;
public sealed class ResumeModule(IAudioService audioService, ILogger<ResumeModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Resumes the music asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
    public async Task ResumeAsync()
    {
        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        if (player.State is not PlayerState.Paused)
        {
            await RespondAsync("Player is not paused.").ConfigureAwait(false);
            return;
        }

        await player.ResumeAsync().ConfigureAwait(false);
        await RespondAsync("Resumed.").ConfigureAwait(false);
    }
}