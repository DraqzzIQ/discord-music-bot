namespace DMusicBot.Dtos;

public record struct PlaylistDto
{
    public string Title { get; init; }
    public string? Author { get; init; }
    public string? Url { get; init; }
    public Uri? ArtworkUrl { get; init; }
    public int? TrackCount { get; init; }
    public TrackDto? SelectedTrack { get; set; }
}