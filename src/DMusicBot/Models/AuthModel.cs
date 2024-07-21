using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DMusicBot.Models;

[BsonIgnoreExtraElements]
public struct AuthModel
{
    [BsonRepresentation(BsonType.String)]
    public Guid Token { get; init; }
    public ulong UserId { get; init; }
    public ulong GuildId { get; init; }
}