namespace DiscordMusicBot.Dtos;

public record struct UserPlaylistPreviewDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string OwnerUsername { get; init; }
    public string OwnerAvatarUrl { get; init; }
    public bool IsPublic { get; init; }
    public bool IsOwn { get; init; }
    public bool IsPinned { get; init; }
    public List<Uri?> PreviewUrls { get; init; }
}