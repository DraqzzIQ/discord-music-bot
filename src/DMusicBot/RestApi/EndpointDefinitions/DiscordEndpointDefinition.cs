using Discord;
using Discord.WebSocket;
using DMusicBot.Dtos;
using DMusicBot.Models;
using DMusicBot.RestApi.Requests;
using DMusicBot.RestApi.Requests.Bot;
using DMusicBot.RestApi.Responses.Discord;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DMusicBot.RestApi.EndpointDefinitions;

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
        UserModel? userModel = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        if (userModel is null)
            return Results.NotFound();
        
        
        List<GuildDto> guilds = [];
        foreach (ulong guildId in userModel.Value.GuildIds)
        {
            IGuild? guild = request.DiscordSocketClient.GetGuild(guildId);
            if (guild is null)
                continue;
            
            guilds.Add(new GuildDto
            {
                Id = guild.Id.ToString(),
                Name = guild.Name,
                IconUrl = guild.IconUrl,
            });
        }
        
        GuildsResponse response = new() {Guilds = guilds.ToArray()};

        return Results.Ok(response);
    }
}