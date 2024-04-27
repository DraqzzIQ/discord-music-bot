using Discord;

namespace DMusicBot.Util;
internal static class EmbedCreator
{
    public static Embed CreateEmbed(string title, string description, Color color, bool timestamp = true, Uri? thumbnailUrl = null)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithColor(color);
        if (timestamp)
            embed.WithCurrentTimestamp();
        if (thumbnailUrl is not null)
            embed.WithThumbnailUrl(thumbnailUrl.ToString());

        return embed.Build();
    }
}
