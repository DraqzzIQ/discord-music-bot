using MongoDB.Bson.Serialization.Attributes;

namespace DMusicBot.Models;

[BsonIgnoreExtraElements]
public struct PlaylistModel
{
    public string Name { get; set; }
    public ulong OwnerId { get; set; }
    public ulong GuildId { get; set; }
    public bool IsPublic { get; set; }
    public List<TrackModel> Tracks { get; set; }
}