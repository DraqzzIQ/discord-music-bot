namespace DiscordMusicBot.Dtos;

public record struct GuildDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string IconUrl { get; init; }
}