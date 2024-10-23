using MongoDB.Bson.Serialization.Attributes;

namespace DiscordMusicBot.Models;

[BsonIgnoreExtraElements]
public struct BotChannelModel
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}