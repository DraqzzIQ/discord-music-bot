using DMusicBot.AudioEventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DMusicBot.Extensions;

public static class HostExtensions
{
    public static void UseAudioEventHandlers(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.Services.GetRequiredService<AudioServiceEventHandler>().RegisterHandlers();
    }
}