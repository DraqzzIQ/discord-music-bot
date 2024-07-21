using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DMusicBot.Models;
using DMusicBot.Services;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;

public class SetBotChannelModule(IAudioService audioService, ILogger<SetBotChannelModule> logger, IDbService dbService) : BaseModule(audioService, logger)
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