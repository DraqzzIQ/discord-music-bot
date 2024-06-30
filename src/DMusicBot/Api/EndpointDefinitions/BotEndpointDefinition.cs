using DMusicBot.Api.Requests;
using DMusicBot.Api.Requests.Bot;
using DMusicBot.Api.Responses.Bot;
using DMusicBot.Services;
using Lavalink4NET;
using Lavalink4NET.Integrations.LyricsJava;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DMusicBot.Api.EndpointDefinitions;

public class BotEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
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
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDbService, MongoDbService>();
    }
    
    private async Task<IResult?> PlayTrackAsync([AsParameters] PlayTrackRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        if(request.Tracks.Length == 0)
        {
            return Results.BadRequest("No tracks provided");
        }
        // Single track
        if(request.Tracks.Length == 1)
        {
            await player.PlayAsync(request.Tracks[0]);
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
        
        return Results.Ok();
    }
    
    private async Task<IResult?> ShuffleQueueAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.Queue.ShuffleAsync();
        
        return Results.Ok();
    }
    
    private async Task<IResult?> PauseAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }
        
        await player.PauseAsync();

        return Results.Ok();
    }
    
    private async Task<IResult?> StopAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.StopAsync();
        
        return Results.Ok();
    }
    
    private async Task<IResult?> ResumeAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.ResumeAsync();
        
        return Results.Ok();
    }
    
    private async Task<IResult?> RewindTrackAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.SeekAsync(TimeSpan.Zero);

        return Results.Ok();
    }
    
    private async Task<IResult?> SkipTrackAsync([AsParameters] BaseRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.SkipAsync();
        
        return Results.Ok();
    }
    
    private async Task<IResult?> UpdateQueueAsync([AsParameters] UpdateQueueRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.Queue.ClearAsync();
        await player.Queue.AddRangeAsync(request.Queue);
        
        return Results.Ok();
    }
    
    private async Task<IResult?> UpdateRepeatModeAsync([AsParameters] UpdateRepeatModeRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        player.RepeatMode = request.RepeatMode;
        
        return Results.Ok();
    }
    
    private async Task<IResult?> UpdateVolumeAsync([AsParameters] UpdateVolumeRequest request)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService);
        if (player is null)
        {
            return Results.NotFound("Player not found");
        }

        await player.SetVolumeAsync(request.Volume / 100f);
        
        return Results.Ok();
    }
    
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
}