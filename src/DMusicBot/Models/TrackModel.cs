using MongoDB.Bson;

namespace DMusicBot.Models;

public struct TrackModel
{
    public ObjectId Id { get; set; }
    public string Title { get; init; }
    public string Author { get; init; }
    public string? Isrc { get; init; }
    public string Identifier { get; init; }
    public string? SourceName { get; init; }
    public Uri? Uri { get; init; }
    public Uri? ArtworkUri { get; init; }
    public TimeSpan Duration { get; init; }
    public string SerializationString { get; init; }
}