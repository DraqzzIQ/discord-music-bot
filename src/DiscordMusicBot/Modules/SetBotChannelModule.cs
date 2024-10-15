using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordMusicBot.Models;
using DiscordMusicBot.Services;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public class SetBotChannelModule(IAudioService audioService, ILogger<SetBotChannelModule> logger, IDbService dbService, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    private readonly IDbService _dbService = dbService;
    
    /// <summary>
    ///     Sets the channel for text messages asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("set-bot-channel", description: "Sets the channel which the bot uses to the current channel", runMode: RunMode.Async)]
    public async Task SetBotChannel()
    {
        SocketGuild? guild = Context.Guild;

        ISocketMessageChannel? channel = Context.Channel;

        if (channel is null)
        {
            await RespondAsync("This command must be used in a channel!").ConfigureAwait(false);
            return;
        }

        await _dbService.SetBotChannelAsync(new BotChannelModel()
        {
            GuildId = guild.Id,
            ChannelId = channel.Id
        }).ConfigureAwait(false);

        await RespondAsync($"Bot channel set to **{channel.Name}**").ConfigureAwait(false);
    }
}