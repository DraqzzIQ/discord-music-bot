namespace DMusicBot.Dtos;

public record struct SearchResponseDto
{
    public TrackDto[] Tracks { get; init; }
    public PlaylistDto[] Playlists { get; init; }
    public PlaylistDto[] Albums { get; init; }
    public string? SearchMode { get; set; }
}