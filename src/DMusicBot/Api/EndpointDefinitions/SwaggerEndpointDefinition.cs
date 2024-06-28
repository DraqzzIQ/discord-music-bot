using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DMusicBot.Api.EndpointDefinitions;

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
        services.AddSwaggerGen();
    }
}