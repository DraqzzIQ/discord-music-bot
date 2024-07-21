using Discord;
using Discord.WebSocket;
using DMusicBot.Api.Responses.Bot;
using DMusicBot.Models;
using DMusicBot.RestApi.Requests;
using DMusicBot.RestApi.Requests.Bot;
using DMusicBot.Services;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Lavalink4NET.Integrations.LyricsJava;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DMusicBot.RestApi.EndpointDefinitions;

public class BotEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/bot/test-auth", TestAuthAsync);

        app.MapGet("/api/bot/status", GetStatusAsync);
        app.MapGet("/api/bot/queue", GetQueueAsync);
        app.MapGet("/api/bot/lyrics", GetLyricsAsync);

        app.MapPut("/api/bot/volume", UpdateVolumeAsync);
        app.MapPut("/api/bot/repeat-mode", UpdateRepeatModeAsync);
        app.MapPut("/api/bot/queue", UpdateQueueAsync);

        app.MapPost("/api/bot/skip", SkipTrackAsync);
        app.MapPost("/api/bot/rewind", RewindTrackAsync);
        app.MapPost("/api/bot/stop", StopAsync);
        app.MapPost("/api/bot/resume", ResumeAsync);
        app.MapPost("/api/bot/pause", PauseAsync);
        app.MapPost("/api/bot/shuffle", ShuffleQueueAsync);
        app.MapPost("/api/bot/play", PlayTrackAsync);
        app.MapPost("/api/bot/join", JoinAsync);
        app.MapPost("/api/bot/leave", LeaveAsync);
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDbService, MongoDbService>();
    }

    [Authorize]
    private async Task<IResult?> TestAuthAsync([AsParameters] BaseRequest request)
    {
        return Results.Ok($"GuildId: {request.GuildId} UserId: {request.UserId}");
    }
    
    [Authorize]
    private async Task<IResult?> LeaveAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.DisconnectAsync();
        
        await SendMessageWithUserPrefixAsync("disconnected the bot", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> JoinAsync([AsParameters] JoinRequest request)
    {
        SocketGuild guild = request.DiscordSocketClient.GetGuild(request.GuildId);
        if (guild is null)
            return Results.NotFound("Guild not found");

        IGuildUser? user = guild.GetUser(request.UserId);
        if (user is null)
            return Results.NotFound("User not found");
        
        if(user.VoiceChannel is null)
            return Results.NotFound("User not in a voice channel");

        if(user.VoiceChannel.GuildId != request.GuildId)
            return Results.NotFound("User not in a voice channel on current guild");

        await request.AudioService.Players.JoinAsync(request.GuildId, user.VoiceChannel.Id, PlayerFactory.Queued);
        
        await SendMessageWithUserPrefixAsync("joined the bot", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> PlayTrackAsync([AsParameters] PlayTrackRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        if (request.Tracks.Length == 0)
        {
            return Results.BadRequest("No tracks provided");
        }

        // Single track
        if (request.Tracks.Length == 1)
        {
            LavalinkTrack track = request.Tracks[0];
            
            Embed embed = EmbedCreator.CreateEmbed($"Added to queue", $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {track.Duration}", Color.Blue, true, track.ArtworkUri);
            await SendEmbedMessageWithUserPrefixAsync("added to queue", embed, request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);
            
            await player.PlayAsync(track);

            return Results.Ok();
        }

        // Playlist
        if (player.CurrentTrack is null)
        {
            await player.PlayAsync(request.Tracks[0]);
            await player.Queue.AddRangeAsync(request.Tracks.Skip(1).Select(t => new TrackQueueItem(t)).ToList());
        }
        else
        {
            await player.Queue.AddRangeAsync(request.Tracks.Select(t => new TrackQueueItem(t)).ToList());
        }
        
        await SendMessageWithUserPrefixAsync($"added {request.Tracks.Length} tracks to the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ShuffleQueueAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.Queue.ShuffleAsync();

        await SendMessageWithUserPrefixAsync("shuffled the queue.", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> PauseAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.PauseAsync();

        await SendMessageWithUserPrefixAsync("paused playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> StopAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.StopAsync();
        
        await SendMessageWithUserPrefixAsync("stopped playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ResumeAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.ResumeAsync();

        await SendMessageWithUserPrefixAsync("resumed playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);
        
        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> RewindTrackAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.SeekAsync(TimeSpan.Zero);
        
        await SendMessageWithUserPrefixAsync("rewound the track", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> SkipTrackAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await SendMessageWithUserPrefixAsync("skipped the track", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        await player.SkipAsync();

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> UpdateQueueAsync([AsParameters] UpdateQueueRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.Queue.ClearAsync();
        await player.Queue.AddRangeAsync(request.Queue);
        
        await SendMessageWithUserPrefixAsync("updated the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> UpdateRepeatModeAsync([AsParameters] UpdateRepeatModeRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        player.RepeatMode = request.RepeatMode;
        
        await SendMessageWithUserPrefixAsync($"set repeat mode to {request.RepeatMode}", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> UpdateVolumeAsync([AsParameters] UpdateVolumeRequest request)
    {
        return Results.Conflict("Disabled");

        // QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        // if (player is null)
        // {
        //     return Results.NotFound("Player not found");
        // }
        //
        // await player.SetVolumeAsync(request.Volume / 100f);
        //
        // await SendMessageWithUserPrefixAsync($"set volume to {request.Volume}", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient);
        //
        // return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> GetLyricsAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        if (player.CurrentTrack is null)
        {
            return Results.NotFound();
        }

        Lyrics? lyrics = await request.AudioService.Tracks.GetCurrentTrackLyricsAsync(player);
        if (lyrics is null)
        {
            return Results.NotFound("Player not found");
        }

        LyricsResponse response = new()
        {
            Lyrics = lyrics
        };

        return Results.Ok(response);
    }

    [Authorize]
    private async Task<IResult?> GetQueueAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        QueueResponse response = new()
        {
            Queue = player.Queue.ToArray()
        };

        return Results.Ok(response);
    }

    [Authorize]
    private async Task<IResult?> GetStatusAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        StatusResponse response = new()
        {
            State = player.State,
            Position = player.Position?.Position ?? TimeSpan.Zero,
            Volume = player.Volume,
            RepeatMode = player.RepeatMode,
            Track = player.CurrentTrack
        };

        return Results.Ok(response);
    }

    private async Task<QueuedLavalinkPlayer?> GetPlayerAsync(ulong guildId, IAudioService audioService)
    {
        var player = await audioService.Players.GetPlayerAsync<QueuedLavalinkPlayer>(guildId);
        if (player is null || player.State == PlayerState.Destroyed)
        {
            return null;
        }

        return player;
    }
    
    private async Task SendMessageWithUserPrefixAsync(string message, ulong guildId, ulong userId, IDbService dbService, DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId);
        if (channel is null)
            return;
        
        await channel.SendMessageAsync($"<@{user.Id}> {message}.", allowedMentions: AllowedMentions.None);
    }
    
    private async Task SendEmbedMessageWithUserPrefixAsync(string message, Embed embed, ulong guildId, ulong userId, IDbService dbService, DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId);
        if (channel is null)
            return;
        
        await channel.SendMessageAsync($"<@{user.Id}> {message}.", embed: embed, allowedMentions: AllowedMentions.None);
    }
}