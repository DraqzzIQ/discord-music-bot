namespace DiscordMusicBot.Dtos;

public record struct TrackDto
{
    public string Title { get; init; }
    public string Author { get; init; }
    public int DurationInSeconds { get; init; }
    public Uri? ThumbnailUrl { get; init; }
    public Uri? Url { get; init; }
    public string? EncodedTrack { get; init; }
}