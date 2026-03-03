using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Title = "DiscordMusicBot API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Token", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = "authCookie",
                Description = "Token Authentication via cookie",
                Scheme = "CustomToken"
            });

            options.AddSecurityDefinition("HeaderAuth", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Token Authentication via Authorization header",
                Scheme = "CustomToken"
            });

            options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement {
                { new OpenApiSecuritySchemeReference("Token"), new List<string>() },
                { new OpenApiSecuritySchemeReference("HeaderAuth"), new List<string>() }
            });
        });
    }
}