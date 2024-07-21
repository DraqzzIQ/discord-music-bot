using MongoDB.Bson.Serialization.Attributes;

namespace DMusicBot.Models;

[BsonIgnoreExtraElements]
public struct BotChannelModel
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}