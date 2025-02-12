using DiscordMusicBot.RestApi.EndpointDefinitions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.Extensions;

public static class EndpointDefinitionExtension
{
    public static void AddEndpointDefinitions(this IServiceCollection services, params Type[] scanMarkers)
    {
        List<IEndpointDefinition> endpointDefinitions = [];

        foreach (var marker in scanMarkers)
        {
            endpointDefinitions.AddRange(
                marker.Assembly.ExportedTypes
                    .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) &&
                                x is { IsInterface: false, IsAbstract: false })
                    .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
            );

            foreach (var endpointDefinition in endpointDefinitions) endpointDefinition.DefineServices(services);

            services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
        }
    }

    public static void UseEndpointDefinitions(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

        foreach (var endpointDefinition in definitions) endpointDefinition.DefineEndpoints(app);
    }
}