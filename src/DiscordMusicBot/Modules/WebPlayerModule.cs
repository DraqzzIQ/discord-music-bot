using Discord;
using Discord.Interactions;
using DiscordMusicBot.Models;
using DiscordMusicBot.Services;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;
public sealed class WebPlayerModule(IAudioService audioService, ILogger<SkipModule> logger, IDbService dbService, ConfigService configService, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    private readonly IDbService _dbService = dbService;
    private readonly ConfigService _configService = configService;
    
    /// <summary>
    ///     Generates a URL with auth token to the web player.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("web-player", description: "Generates a link to the web interface", runMode: RunMode.Async)]
    public async Task WebPlayer()
    {
        await DeferAsync(ephemeral: true).ConfigureAwait(false);
        
        Guid token = SecureGuidGenerator.CreateCryptographicallySecureGuid();
        
        UserModel? userModel = await _dbService.GetUserAsync(Context.User.Id).ConfigureAwait(false);
        if (userModel is not null)
        {
            UserModel updatedUser = userModel.Value;
            updatedUser.Token = token; // Regenerate token
            if(!updatedUser.GuildIds.Contains(Context.Guild.Id))
                updatedUser.GuildIds = updatedUser.GuildIds.Append(Context.Guild.Id).ToArray(); // Add guild to user
            await _dbService.UpdateUserAsync(updatedUser).ConfigureAwait(false);
        }
        else
        {
            UserModel user = new()
            {
                GuildIds = new[] {Context.Guild.Id},
                UserId = Context.User.Id,
                Token = token
            };
        
            await _dbService.AddUserAsync(user).ConfigureAwait(false);
        }
       
        Embed embed = new EmbedBuilder()
            .WithTitle("Web Player")
            .WithDescription($"[Click here to open the web player]({configService.FrontendBaseUrl}/api/login/{token})")
            .Build();
        
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }
}