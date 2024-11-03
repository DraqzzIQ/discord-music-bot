using Discord;
using DiscordMusicBot.Dtos;
using DiscordMusicBot.RestApi.Requests;
using DiscordMusicBot.RestApi.Responses.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class DiscordEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/discord/guilds", GetGuildsAsync);
    }

    public void DefineServices(IServiceCollection services)
    {
    }

    [Authorize]
    private async Task<IResult?> GetGuildsAsync([AsParameters] BaseRequest request)
    {
        var userModel = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        if (userModel is null)
            return Results.NotFound();


        List<GuildDto> guilds = [];
        foreach (var guildId in userModel.Value.GuildIds)
        {
            IGuild? guild = request.DiscordSocketClient.GetGuild(guildId);
            if (guild is null)
                continue;

            guilds.Add(new GuildDto
            {
                Id = guild.Id.ToString(),
                Name = guild.Name,
                IconUrl = guild.IconUrl
            });
        }

        GuildsResponse response = new() { Guilds = guilds.ToArray() };

        return Results.Ok(response);
    }
}