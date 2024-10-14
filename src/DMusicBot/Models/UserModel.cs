using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DMusicBot.Models;

[BsonIgnoreExtraElements]
public struct UserModel
{
    [BsonRepresentation(BsonType.String)]
    public Guid Token { get; set; }
    public ulong UserId { get; init; }
    public ulong[] GuildIds { get; set; }
}