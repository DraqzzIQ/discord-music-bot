using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;

public class JoinModule(IAudioService audioService, ILogger<LoopModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Loops the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("join", description: "Joins the voice channel.", runMode: RunMode.Async)]
    public async Task LoopAsync()
    {
        await DeferAsync().ConfigureAwait(false);
        
        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);
        if(player is null)
        {
            return;
        }
        
        await FollowupAsync("Joined the voice channel").ConfigureAwait(false);
    }
}