using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DMusicBot;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET;

Config.ReadConfiguration();

var builder = new HostApplicationBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordClientHost>();

// Lavalink
builder.Services.AddLavalink();
builder.Services.AddInactivityTracking();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Warning));


builder.Services.ConfigureLavalink(options =>
{
    options.Passphrase = Config.Data.LL_Password;
    options.BaseAddress = new Uri("http://" + Config.Data.LL_Hostname + ":" + Config.Data.LL_Port);
    options.ReadyTimeout = TimeSpan.FromSeconds(10);
    options.HttpClientName = "LavalinkHttpClient";
    options.Label = "DMusicBot";
});

builder.Services.Configure<UsersInactivityTrackerOptions>(options =>
{
    options.Threshold = 1;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.ExcludeBots = true;
});

var app = builder.Build();
app.UseLyricsJava();

IAudioService? audioService = app.Services.GetService<IAudioService>();
AudioServiceEventHandler.RegisterHandlers(audioService!);

app.Run();