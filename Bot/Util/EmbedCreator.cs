using Discord;

namespace DMusicBot.Util;

public static class EmbedCreator
{
    private const int MaxEmbedMessageSize = 4000;
    private const int MaxTotalMessageSize = 5800; // Buffer for embed fields
    private const int MaxEmbedsPerMessage = 10;

    public static Embed CreateEmbed(string title, string description, Color? color = null, bool timestamp = true, Uri? thumbnailUrl = null)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(description);
        if (color.HasValue)
            embed.WithColor(color.Value);
        if (timestamp)
            embed.WithCurrentTimestamp();
        if (thumbnailUrl is not null)
            embed.WithThumbnailUrl(thumbnailUrl.ToString());

        return embed.Build();
    }

    public static Embed[] CreateEmbeds(string title, string description, ushort maxCharsPerEmbed = MaxEmbedMessageSize, Color? color = null,
        bool timestamp = true, Uri? thumbnailUrl = null)
    {
        List<Embed> chunks = [];
        int totalLength = 0;

        while (!string.IsNullOrWhiteSpace(description) && totalLength < MaxTotalMessageSize)
        {
            int length = Math.Min(description.Length, Math.Min(maxCharsPerEmbed, MaxTotalMessageSize - totalLength));
            string chunk = description[..length];
            description = description[length..];
            totalLength += length;

            var embed = new EmbedBuilder()
                .WithDescription(chunk);
            if (chunks.Count == 0)
                embed.WithTitle(title);
            if (color.HasValue)
                embed.WithColor(color.Value);
            // Add timestamp to the last embed
            if (timestamp && (description.Length == 0 || totalLength == MaxTotalMessageSize || chunks.Count == MaxEmbedsPerMessage - 1))
                embed.WithCurrentTimestamp();
            if (thumbnailUrl is not null && chunks.Count == 0)
                embed.WithThumbnailUrl(thumbnailUrl.ToString());

            chunks.Add(embed.Build());
            if (chunks.Count == MaxEmbedsPerMessage)
                break;
        }

        return chunks.ToArray();
    }

    public static Embed[] CreateEmbeds(string title, List<string> descriptionLines, ushort maxCharsPerEmbed = MaxEmbedMessageSize, Color? color = null,
        bool timestamp = true, Uri? thumbnailUrl = null)
    {
        List<Embed> chunks = [];
        int totalLength = 0;
        int index = 0;

        while (index < descriptionLines.Count && totalLength + descriptionLines[index].Length < MaxTotalMessageSize)
        {
            string chunk = "";
            while (index < descriptionLines.Count && chunk.Length + descriptionLines[index].Length < maxCharsPerEmbed &&
                   totalLength + descriptionLines[index].Length < MaxTotalMessageSize)
            {
                chunk += descriptionLines[index] + "\n";
                totalLength += descriptionLines[index].Length;
                index++;
            }

            var embed = new EmbedBuilder()
                .WithDescription(chunk);
            if (chunks.Count == 0)
                embed.WithTitle(title);
            if (color.HasValue)
                embed.WithColor(color.Value);
            // Add timestamp to the last embed
            if (timestamp && (index >= descriptionLines.Count || totalLength + descriptionLines[index].Length > MaxTotalMessageSize ||
                              chunks.Count == MaxEmbedsPerMessage - 1))
                embed.WithCurrentTimestamp();
            if (thumbnailUrl is not null && chunks.Count == 0)
                embed.WithThumbnailUrl(thumbnailUrl.ToString());

            chunks.Add(embed.Build());
            if (chunks.Count == MaxEmbedsPerMessage)
                break;
        }

        return chunks.ToArray();
    }
}