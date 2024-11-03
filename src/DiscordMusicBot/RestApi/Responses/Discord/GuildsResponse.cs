using DiscordMusicBot.Dtos;

namespace DiscordMusicBot.RestApi.Responses.Discord;

public class GuildsResponse
{
    public GuildDto[] Guilds { get; init; }
}