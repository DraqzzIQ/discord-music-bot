using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class CompressionDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.UseResponseCompression();
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
    }
}