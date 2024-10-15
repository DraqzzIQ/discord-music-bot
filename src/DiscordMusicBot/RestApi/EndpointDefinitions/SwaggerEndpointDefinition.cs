using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
        if (!services.BuildServiceProvider().GetRequiredService<IHostEnvironment>().IsDevelopment()) return;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new() { Title = "DiscordMusicBot API", Version = "v1" });
                // Auth token in the authCookie cookie
                options.AddSecurityDefinition("Token", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Cookie,
                    Name = "authCookie", // The name of the cookie to be used
                    Description = "Token Authentication via cookie",
                    Scheme = "CustomToken"
                });

                // Auth token in the Authorization header
                options.AddSecurityDefinition("HeaderAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization", // The name of the header to be used
                    Description = "Token Authentication via Authorization header",
                    Scheme = "CustomToken"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Token"
                            },
                            Scheme = "Token",
                            Name = "Token",
                            In = ParameterLocation.Cookie,
                        },

                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "HeaderAuth"
                            },
                            Scheme = "CustomToken",
                            Name = "HeaderAuth",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            }
        );
    }
}