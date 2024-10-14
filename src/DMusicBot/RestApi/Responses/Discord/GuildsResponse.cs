using Discord;
using DMusicBot.Dtos;

namespace DMusicBot.RestApi.Responses.Discord;

public class GuildsResponse
{
    public GuildDto[] Guilds { get; init; }
}