using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using Lavalink4NET.InactivityTracking.Extensions;
using Lavalink4NET.InactivityTracking.Trackers.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Discord.Rest;
using DiscordMusicBot.Audio;
using DiscordMusicBot.Extensions;
using DiscordMusicBot.RestApi.Auth;
using DiscordMusicBot.RestApi.EndpointDefinitions;
using DiscordMusicBot.Services;
using Lavalink4NET.InactivityTracking.Trackers.Idle;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Information));
#endif

// Lavalink
builder.Services.AddLavalink();

builder.Services.ConfigureLavalink(options =>
{
    options.Passphrase = ConfigService.LavaLinkPassword;
    options.BaseAddress = new Uri(ConfigService.LavaLinkConnectionString);
    options.ReadyTimeout = TimeSpan.FromSeconds(10);
    options.HttpClientName = "LavalinkHttpClient";
    options.Label = "DiscordMusicBot";
});

// Inactivity Tracking
builder.Services.AddInactivityTracking();
builder.Services.AddInactivityTracker<UsersInactivityTracker>();
builder.Services.AddInactivityTracker<IdleInactivityTracker>();

builder.Services.ConfigureInactivityTracking(options =>
{
});

builder.Services.Configure<UsersInactivityTrackerOptions>(options =>
{
    options.Threshold = 1;
    options.Timeout = TimeSpan.FromSeconds(180);
    options.ExcludeBots = true;
});

builder.Services.Configure<IdleInactivityTrackerOptions>(options =>
{
    options.Timeout = TimeSpan.FromSeconds(1800);
});

// Audio Service Event Handler
builder.Services.AddSingleton<AudioServiceEventHandler>();

// Api
builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition));
builder.Services.AddSingleton(TimeProvider.System);

// DB
builder.Services.AddSingleton<IDbService, MongoDbService>();


// Auth
builder.Services.AddAuthentication
        (CustomAuthenticationSchemeOptions.DefaultScheme)
    .AddScheme<CustomAuthenticationSchemeOptions, CustomAuthenticationHandler>
    (CustomAuthenticationSchemeOptions.DefaultScheme,
        options => { });


builder.Services.AddAuthorization();

var app = builder.Build();

// Lyrics
app.UseLyricsJava();

// Api
app.UseHttpsRedirection();
app.UseEndpointDefinitions();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Audio Event Handlers
app.UseAudioEventHandlers();

app.Run();