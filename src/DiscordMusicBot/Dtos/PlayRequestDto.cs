namespace DiscordMusicBot.Dtos;

public record struct PlayRequestDto
{
    public bool IsPlaylist { get; init; }
    public bool IsTrack => !IsPlaylist;
    public bool ShouldPlay { get; init; }
    public bool ShouldEnqueue => !ShouldPlay;
    public string? EncodedTrack { get; init; }
    public string? PlaylistUrl { get; init; }
    public string[]? EncodedPlaylistTracks { get; init; }
}