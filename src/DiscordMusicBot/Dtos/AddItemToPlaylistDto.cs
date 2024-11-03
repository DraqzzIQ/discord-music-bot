namespace DiscordMusicBot.Dtos;

public record struct AddItemToPlaylistDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public bool ContainsItem { get; init; }
    public List<Uri?> PreviewUrls { get; init; }
}