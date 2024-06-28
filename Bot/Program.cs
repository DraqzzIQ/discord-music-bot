using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DMusicBot;
using Lavalink4NET;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Discord.Rest;
using DMusicBot.Common.Services;
using DMusicBot.Services;

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>();
// tmp fix:
builder.Services.AddSingleton<IRestClientProvider>(x => x.GetRequiredService<DiscordSocketClient>());
// end tmp fix
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordClientHost>();

// Logging
#if DEBUG
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Debug));
#else
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Warning));
#endif

// Config
builder.Services.AddSingleton<ConfigService, BotConfigService>();

// Lavalink
builder.Services.AddLavalink();
builder.Services.AddInactivityTracker<UsersInactivityTracker>();

BotConfigService config = builder.Build().Services.GetRequiredService<ConfigService>() as BotConfigService ?? throw new InvalidOperationException("Config service is not of type BotConfigService");
builder.Services.ConfigureLavalink(options =>
{
    options.Passphrase = config.LavaLinkPassword;
    options.BaseAddress = new Uri(config.LavaLinkConnectionString);
    options.ReadyTimeout = TimeSpan.FromSeconds(10);
    options.HttpClientName = "LavalinkHttpClient";
    options.Label = "DMusicBot";
});

builder.Services.Configure<UsersInactivityTrackerOptions>(options =>
{
    options.Threshold = 1;
    options.Timeout = TimeSpan.FromSeconds(180);
    options.ExcludeBots = true;
});

// Db Service
builder.Services.AddSingleton<IDbService, MongoDbService>();


var app = builder.Build();

// Lyrics
app.UseLyricsJava();

IAudioService? audioService = app.Services.GetService<IAudioService>();
AudioServiceEventHandler.RegisterHandlers(audioService!);

app.Run();