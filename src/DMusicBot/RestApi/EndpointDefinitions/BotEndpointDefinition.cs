using System.Collections.Immutable;
using Discord;
using Discord.WebSocket;
using DMusicBot.Audio;
using DMusicBot.Dtos;
using DMusicBot.Extensions;
using DMusicBot.Factories;
using DMusicBot.Models;
using DMusicBot.RestApi.Requests;
using DMusicBot.RestApi.Requests.Bot;
using DMusicBot.RestApi.Responses.Bot;
using DMusicBot.Services;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Integrations.LyricsJava;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DMusicBot.RestApi.EndpointDefinitions;

public class BotEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.MapGet("/api/bot/test-auth", TestAuthAsync);

        app.MapGet("/api/bot/lyrics", GetLyricsAsync);
        app.MapGet("/api/bot/search", GetSearchAsync);

        app.MapPost("/api/bot/position", UpdatePositionAsync);
        app.MapPost("/api/bot/reorder", ReorderQueueAsync);
        app.MapPost("/api/bot/remove", RemoveFromQueueAsync);
        app.MapPost("/api/bot/clear", ClearQueueAsync);
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
    }

    [Authorize]
    private async Task<IResult?> TestAuthAsync([AsParameters] BaseRequest request)
    {
        UserModel? user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        if (user is null)
            return Results.NotFound("User not found");

        //return Results.Ok($"GuildIds: {string.Join(',', user.Value.GuildIds)} UserId: {request.UserId}");
        return Results.Ok("authenticated");
    }

    [Authorize]
    private async Task<IResult?> UpdatePositionAsync([AsParameters] UpdatePlayerPositionRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        if (player.CurrentTrack is null)
            return Results.NotFound("No track playing");

        TimeSpan position = TimeSpan.FromSeconds(request.PositionInSeconds);

        await player.SeekSignalRAsync(position).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync($"seeked to {TimeSpanFormatter.FormatDuration(position)}", request.GuildId, request.UserId, request.DbService,
            request.DiscordSocketClient).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> LeaveAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.DisconnectAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("disconnected the bot", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> JoinAsync([AsParameters] JoinRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SocketGuild guild = request.DiscordSocketClient.GetGuild(request.GuildId);
        if (guild is null)
            return Results.NotFound("Guild not found");

        IGuildUser? user = guild.GetUser(request.UserId);
        if (user is null)
            return Results.NotFound("User not found");

        if (user.VoiceChannel is null)
            return Results.NotFound("User not in a voice channel");

        if (user.VoiceChannel.GuildId != request.GuildId)
            return Results.NotFound("User not in a voice channel on current guild");

        await request.AudioService.Players.JoinAsync(request.GuildId, user.VoiceChannel.Id, CustomQueuedPlayerFactory.CustomQueued).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("joined the bot", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> PlayTrackAsync([AsParameters] PlayTrackRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        if (request.Tracks.Length == 0)
            return Results.BadRequest("No tracks provided");

        // Single track
        if (request.Tracks.Length == 1)
        {
            LavalinkTrack track = request.Tracks[0];

            Embed embed = EmbedCreator.CreateEmbed($"Added to queue", $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {track.Duration}", Color.Blue,
                true, track.ArtworkUri);
            await SendEmbedMessageWithUserPrefixAsync("added to queue", embed, request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
                .ConfigureAwait(false);

            await player.PlayAsync(track).ConfigureAwait(false);

            return Results.Ok();
        }

        // Playlist
        if (player.CurrentTrack is null)
        {
            await player.PlayAsync(request.Tracks[0]).ConfigureAwait(false);
            await player.Queue.AddRangeAsync(request.Tracks.Skip(1).Select(t => new TrackQueueItem(t)).ToList()).ConfigureAwait(false);
        }
        else
        {
            await player.Queue.AddRangeAsync(request.Tracks.Select(t => new TrackQueueItem(t)).ToList()).ConfigureAwait(false);
        }

        await SendMessageWithUserPrefixAsync($"added {request.Tracks.Length} tracks to the queue", request.GuildId, request.UserId, request.DbService,
            request.DiscordSocketClient).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ShuffleQueueAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.ShuffleQueueSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("shuffled the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> PauseAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.PauseSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("paused playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> StopAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        QueuedLavalinkPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.StopAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("stopped playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ResumeAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.ResumeSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("resumed playback", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> RewindTrackAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");


        await player.SeekSignalRAsync(TimeSpan.Zero).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("rewound the track", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ReorderQueueAsync([AsParameters] ReorderQueueRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.ReorderQueueSignalRAsync(request.SourceIndex, request.DestinationIndex).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync($"reordered the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> RemoveFromQueueAsync([AsParameters] RemoveFromQueueRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        if (request.Index < 0 || request.Index >= player.Queue.Count)
            return Results.BadRequest("Invalid index");

        await player.RemoveAtSignalRAsync(request.Index).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync($"removed a track from the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> ClearQueueAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");


        await SendMessageWithUserPrefixAsync("cleared the queue", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        await player.ClearQueueSignalRAsync().ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> SkipTrackAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");


        await SendMessageWithUserPrefixAsync("skipped the track", request.GuildId, request.UserId, request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

        await player.SkipSignalRAsync().ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> GetLyricsAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");


        if (player.CurrentTrack is null)
        {
            return Results.NotFound();
        }

        Lyrics? lyrics = await request.AudioService.Tracks.GetCurrentTrackLyricsAsync(player).ConfigureAwait(false);
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
    private async Task<IResult?> GetSearchAsync([AsParameters] SearchRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        TrackSearchMode mode = request.SearchMode switch
        {
            "ytsearch" => TrackSearchMode.YouTube,
            "ytmsearch" => TrackSearchMode.YouTubeMusic,
            "scsearch" => TrackSearchMode.SoundCloud,
            "spsearch" => TrackSearchMode.Spotify,
            "amsearch" => TrackSearchMode.AppleMusic,
            "dzsearch" => TrackSearchMode.Deezer,
            "ymsearch" => TrackSearchMode.YandexMusic,
            "linksearch" => TrackSearchMode.None,
            _ => TrackSearchMode.None
        };

        // for some reason links don't work with lavasearch
        // yt and ytm don't seam to work with either
        if (mode == TrackSearchMode.None || mode == TrackSearchMode.YouTube || mode == TrackSearchMode.YouTubeMusic)
        {
            var linkResult = await request.AudioService.Tracks.LoadTracksAsync(request.Query, mode).ConfigureAwait(false);
            if(!linkResult.HasMatches)
                return Results.NotFound("No results found");

            if (linkResult.IsPlaylist)
            {
                var playlist = new ExtendedPlaylistInformation(linkResult.Playlist!);
                SearchResponseDto dto = new SearchResponseDto
                {
                    Playlists = [playlist.ToPlaylistDto()],
                    Tracks = [],
                    Albums = []
                };

                if (linkResult.Playlist!.SelectedTrack is null)
                    dto.Playlists[0].SelectedTrack = linkResult.Track?.ToTrackDto();

                return Results.Ok(dto);
            }
            
            return Results.Ok(new SearchResponseDto
            {
                Tracks = [linkResult.Track!.ToTrackDto(true)],
                Playlists = [],
                Albums = []
            });
        }

        var result = await request.AudioService.Tracks.SearchAsync(
            request.Query,
            loadOptions: new TrackLoadOptions(SearchMode: mode),
            categories: ImmutableArray.Create(SearchCategory.Track, SearchCategory.Playlist, SearchCategory.Album)
        ).ConfigureAwait(false);

        if (result is null)
            return Results.NotFound("No results found");

        var playlists = result.Playlists.Select(p => new ExtendedPlaylistInformation(p)).ToArray();
        var albums = result.Albums.Select(a => new ExtendedPlaylistInformation(a)).ToArray();

        SearchResponseDto response = new()
        {
            Tracks = result.Tracks.Select(t => t.ToTrackDto(true)).ToArray(),
            Playlists = playlists.Select(p => p.ToPlaylistDto()).ToArray(),
            Albums = albums.Select(a => a.ToPlaylistDto()).ToArray(),
            SearchMode = mode.Prefix
        };

        return Results.Ok(response);
    }

    private async Task<SignalRPlayer?> GetPlayerAsync(ulong guildId, IAudioService audioService)
    {
        var player = await audioService.Players.GetPlayerAsync<SignalRPlayer>(guildId).ConfigureAwait(false);
        if (player is null || player.State == PlayerState.Destroyed)
        {
            return null;
        }

        return player;
    }

    private async Task SendMessageWithUserPrefixAsync(string message, ulong guildId, ulong userId, IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId).ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", allowedMentions: AllowedMentions.None).ConfigureAwait(false);
    }

    private async Task SendEmbedMessageWithUserPrefixAsync(string message, Embed embed, ulong guildId, ulong userId, IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId).ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", embed: embed, allowedMentions: AllowedMentions.None).ConfigureAwait(false);
    }

    private async Task<bool> EnsureAuthorizedAsync(GuildRequest request)
    {
        UserModel? user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        return user is not null && user.Value.GuildIds.Contains(request.GuildId);
    }
}