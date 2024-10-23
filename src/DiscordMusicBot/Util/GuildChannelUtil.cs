using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Models;
using DiscordMusicBot.Services;

namespace DiscordMusicBot.Util;

public static class GuildChannelUtil
{
    public static async Task<ITextChannel?> GetBotGuildChannel(IDbService dbService, DiscordSocketClient discordSocketClient, ulong guildId)
    {
        ITextChannel? textChannel;
        BotChannelModel? botChannelModel = await dbService.GetBotChannelAsync(guildId).ConfigureAwait(false);
        if (botChannelModel is null || (textChannel = await discordSocketClient.GetChannelAsync(botChannelModel.Value.ChannelId).ConfigureAwait(false) as ITextChannel) is null)
        {
            SocketGuild? guild = discordSocketClient.GetGuild(guildId);
            textChannel = guild?.SystemChannel ?? guild?.TextChannels.FirstOrDefault();
        }

        return textChannel;
    }
}