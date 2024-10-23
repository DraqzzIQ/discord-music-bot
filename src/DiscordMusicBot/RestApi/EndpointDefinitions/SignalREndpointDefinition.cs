using DiscordMusicBot.Services;
using DiscordMusicBot.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class SignalREndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.UseCors("CorsPolicy");
        // Use default files and static files
        app.MapHub<BotHub>("/bot");
    }

    public void DefineServices(IServiceCollection services)
    {
        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(ConfigService.FrontendBaseUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddSignalR();
    }
}