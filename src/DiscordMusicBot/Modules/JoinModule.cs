using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public class JoinModule(IAudioService audioService, ILogger<LoopModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Joins the voice channel.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("join", description: "Joins the voice channel.", runMode: RunMode.Async)]
    public async Task JoinAsync()
    {
        await DeferAsync().ConfigureAwait(false);
        
        var player = await GetPlayerAsync(connectToVoiceChannel: true, updatePlayer: true).ConfigureAwait(false);
        if(player is null)
        {
            return;
        }
        
        await FollowupAsync("Joined the voice channel").ConfigureAwait(false);
    }
}