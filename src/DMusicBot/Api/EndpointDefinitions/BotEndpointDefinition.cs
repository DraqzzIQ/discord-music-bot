using DMusicBot.Api.Requests;
using DMusicBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DMusicBot.Api.EndpointDefinitions;

public class BotEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/bot/status", GetBotStatusAsync);
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDbService, MongoDbService>();
    }
    
    private async Task<IResult?> GetBotStatusAsync()
    {
        return Results.Ok("Bot is running");
    }
}