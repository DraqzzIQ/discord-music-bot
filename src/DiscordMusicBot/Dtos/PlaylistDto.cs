namespace DiscordMusicBot.Dtos;

public record struct PlaylistDto
{
    public string Title { get; init; }
    public string? Author { get; init; }
    public string? Url { get; init; }
    public Uri? ArtworkUrl { get; init; }
    public int? TrackCount { get; set; }
    public TrackDto? SelectedTrack { get; set; }
    public string[]? EncodedTracks { get; set; }
}