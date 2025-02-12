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

public sealed class WebPlayerModule(
    IAudioService audioService,
    ILogger<SkipModule> logger,
    IDbService dbService,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Generates a URL with auth token to the web player.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("web-player", "Generates a link to the web interface", runMode: RunMode.Async)]
    public async Task WebPlayer()
    {
        await DeferAsync(true).ConfigureAwait(false);

        var token = SecureGuidGenerator.CreateCryptographicallySecureGuid();

        var userModel = await dbService.GetUserAsync(Context.User.Id).ConfigureAwait(false);
        if (userModel is not null)
        {
            var updatedUser = userModel.Value;
            updatedUser.Token = token; // Regenerate token
            if (!updatedUser.GuildIds.Contains(Context.Guild.Id))
                updatedUser.GuildIds = updatedUser.GuildIds.Append(Context.Guild.Id).ToArray(); // Add guild to user
            await dbService.UpdateUserAsync(updatedUser).ConfigureAwait(false);
        }
        else
        {
            UserModel user = new()
            {
                GuildIds = [Context.Guild.Id],
                UserId = Context.User.Id,
                Token = token,
                PinnedPlaylists = []
            };

            await dbService.AddUserAsync(user).ConfigureAwait(false);
        }

        var embed = new EmbedBuilder()
            .WithTitle("Web Player")
            .WithDescription($"[Click here to open the web player]({ConfigService.FrontendBaseUrl}/api/login/{token})")
            .Build();

        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }
}