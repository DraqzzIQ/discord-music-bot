using MongoDB.Bson;

namespace DiscordMusicBot.Models;

public class PlaylistModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ulong OwnerId { get; set; }
    public ulong GuildId { get; set; }
    public bool IsPublic { get; set; }
    public List<TrackModel> Tracks { get; set; }
}