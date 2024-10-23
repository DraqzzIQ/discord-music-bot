using System.Collections.Immutable;
using Amazon.SecurityToken;
using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Audio;
using DiscordMusicBot.Dtos;
using DiscordMusicBot.Factories;
using DiscordMusicBot.Models;
using DiscordMusicBot.RestApi.Requests;
using DiscordMusicBot.RestApi.Requests.Bot;
using DiscordMusicBot.RestApi.Responses.Bot;
using DiscordMusicBot.Services;
using DiscordMusicBot.Util;
using DiscordMusicBot.Extensions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop.Infrastructure;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class BotEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
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
        app.MapPost("/api/bot/deduplicate", DeduplicateQueueAsync);
        app.MapPost("/api/bot/play", PlayTrackAsync);
        app.MapPost("/api/bot/leave", LeaveAsync);
    }

    public void DefineServices(IServiceCollection services)
    {
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

        await SendMessageWithUserPrefixAsync($"seeked to {TimeSpanFormatter.FormatDuration(position)}", request.GuildId,
            request.UserId, request.DbService,
            request.DiscordSocketClient).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> LeaveAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player =
            await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.DisconnectSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("disconnected the bot", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> PlayTrackAsync([AsParameters] PlayRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player =
            await GetPlayerAsync(request.GuildId, request.AudioService, true, request.DiscordSocketClient,
                request.UserId, request.HubContext).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        // Single track
        if (request.PlayRequestDto.IsTrack)
        {
            LavalinkTrack track = LavalinkTrack.Parse(request.PlayRequestDto.EncodedTrack!, provider: null);

            Embed embed = EmbedCreator.CreateEmbed($"Added to queue",
                $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {TimeSpanFormatter.FormatDuration(track.Duration)}",
                Color.Blue,
                true, track.ArtworkUri);
            await SendEmbedMessageWithUserPrefixAsync("added to queue", embed, request.GuildId, request.UserId,
                    request.DbService, request.DiscordSocketClient)
                .ConfigureAwait(false);

            if (request.PlayRequestDto.ShouldEnqueue)
                await player.PlaySignalRAsync(track).ConfigureAwait(false);
            else
                await player.PlaySignalRAsync(track, enqueue: false).ConfigureAwait(false);

            return Results.Ok();
        }

        // Playlist
        TrackLoadResult? trackLoadResult = null;
        if (request.PlayRequestDto.PlaylistUrl is not null)
            trackLoadResult = await request.AudioService.Tracks
                .LoadTracksAsync(request.PlayRequestDto.PlaylistUrl, TrackSearchMode.None).ConfigureAwait(false);


        var tracks = request.PlayRequestDto.PlaylistUrl is not null
            ? trackLoadResult!.Value.Tracks.ToArray()
            :
            [
                ..request.PlayRequestDto.EncodedPlaylistTracks!.Select(t =>
                    LavalinkTrack.Parse(t, provider: null))
            ];

        // // Deezer artwork url being weird
        // if (trackLoadResult is not null && trackLoadResult.Value.Tracks.Any(t => t.SourceName == "deezer"))      
        // {
        //     for (int i = 0; i < tracks.Length; i++)
        //     {
        //         if (tracks[i].ArtworkUri is null && tracks[i].AdditionalInformation.ContainsKey("artistArtworkUrl"))
        //         {
        //             tracks[i] = tracks[i] with
        //             {
        //                 ArtworkUri = new Uri(tracks[i].AdditionalInformation["artistArtworkUrl"].ToString())
        //             };
        //         }
        //     }
        // }

        if (request.PlayRequestDto.ShouldEnqueue)
        {
            if (player.CurrentTrack is null)
            {
                await player.PlaySignalRAsync(tracks[0]).ConfigureAwait(false);
                await player.AddRangeSignalRAsync(tracks.Skip(1)).ConfigureAwait(false);
            }
            else
                await player.AddRangeSignalRAsync(tracks).ConfigureAwait(false);
        }
        else
        {
            await player.PlaySignalRAsync(tracks[0], enqueue: false).ConfigureAwait(false);
            await player.InsertRangeSignalRAsync(0, tracks.Skip(1)).ConfigureAwait(false);
        }

        await SendMessageWithUserPrefixAsync($"added {tracks.Length} tracks to the queue", request.GuildId,
            request.UserId, request.DbService,
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

        await SendMessageWithUserPrefixAsync("shuffled the queue", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> DeduplicateQueueAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.DeduplicateQueueSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("deduplicated the queue", request.GuildId, request.UserId,
                request.DbService, request.DiscordSocketClient)
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

        await SendMessageWithUserPrefixAsync("paused playback", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> StopAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player =
            await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        await player.StopSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("stopped playback", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
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

        await SendMessageWithUserPrefixAsync("resumed playback", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
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

        await SendMessageWithUserPrefixAsync("rewound the track", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
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

        await SendMessageWithUserPrefixAsync($"reordered the queue", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
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

        await SendMessageWithUserPrefixAsync($"removed a track from the queue", request.GuildId, request.UserId,
                request.DbService, request.DiscordSocketClient)
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

        await player.ClearQueueSignalRAsync().ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("cleared the queue", request.GuildId, request.UserId, request.DbService,
                request.DiscordSocketClient)
            .ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult?> SkipTrackAsync([AsParameters] SkipToTrackRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        SignalRPlayer? player = await GetPlayerAsync(request.GuildId, request.AudioService).ConfigureAwait(false);
        if (player is null)
            return Results.NotFound("Player not found");

        if (request.Index == 0)
        {
            await player.SkipSignalRAsync().ConfigureAwait(false);

            await SendMessageWithUserPrefixAsync("skipped the track", request.GuildId, request.UserId,
                    request.DbService, request.DiscordSocketClient)
                .ConfigureAwait(false);

            return Results.Ok();
        }

        await player.SkipToTrackSignalRAsync(request.Index).ConfigureAwait(false);

        await SendMessageWithUserPrefixAsync("skipped to a track", request.GuildId, request.UserId,
                request.DbService, request.DiscordSocketClient)
            .ConfigureAwait(false);

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
            var linkResult = await request.AudioService.Tracks.LoadTracksAsync(request.Query, mode)
                .ConfigureAwait(false);
            if (!linkResult.HasMatches)
                return Results.NotFound("No results found");

            if (linkResult.IsPlaylist)
            {
                var playlist = new ExtendedPlaylistInformation(linkResult.Playlist!);
                SearchResponseDto dto = new SearchResponseDto
                {
                    Playlists =
                    [
                        // if the playlist is from youtube or youtubemusic, send the tracks because no link is provided
                        linkResult.Tracks[0].SourceName == "youtube" || linkResult.Tracks[0].SourceName == "youtubemusic"
                            ? playlist.ToPlaylistDto(linkResult.Tracks.Select(t => t.ToString()).ToArray())
                            : playlist.ToPlaylistDto()
                    ],
                    Tracks = [],
                    Albums = []
                };

                if (linkResult.Playlist!.SelectedTrack is null)
                    dto.Playlists[0].SelectedTrack = linkResult.Track?.ToTrackDto();

                if (dto.Playlists[0].TrackCount is null && linkResult.Tracks.Length > 0)
                    dto.Playlists[0].TrackCount = linkResult.Tracks.Length;

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
            Albums = albums.Select(a => a.ToPlaylistDto()).ToArray()
        };

        return Results.Ok(response);
    }

    private async Task<SignalRPlayer?> GetPlayerAsync(ulong guildId, IAudioService audioService, bool join = false,
        DiscordSocketClient? discordSocketClient = null, ulong userId = 0, IHubContext<BotHub, IBotClient>? hubContext = null)
    {
        var player = await audioService.Players.GetPlayerAsync<SignalRPlayer>(guildId).ConfigureAwait(false);
        if (player is null || player.State == PlayerState.Destroyed)
        {
            if (join && discordSocketClient is not null && hubContext is not null)
            {
                SocketGuild? guild = discordSocketClient.GetGuild(guildId);

                IGuildUser? user = guild?.GetUser(userId);

                if (user?.VoiceChannel is null)
                    return null;

                if (user.VoiceChannel.GuildId != guildId)
                    return null;
                
                var options = new SignalRPlayerOptions {HubContext = hubContext};

                return await audioService.Players
                    .JoinAsync(guildId, user.VoiceChannel.Id, CustomQueuedPlayerFactory.CustomQueued, options)
                    .ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        return player;
    }

    private async Task SendMessageWithUserPrefixAsync(string message, ulong guildId, ulong userId, IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId)
            .ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", allowedMentions: AllowedMentions.None)
            .ConfigureAwait(false);
    }

    private async Task SendEmbedMessageWithUserPrefixAsync(string message, Embed embed, ulong guildId, ulong userId,
        IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        SocketUser user = discordSocketClient.GetUser(userId);
        ITextChannel? channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId)
            .ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", embed: embed, allowedMentions: AllowedMentions.None)
            .ConfigureAwait(false);
    }

    private async Task<bool> EnsureAuthorizedAsync(GuildRequest request)
    {
        UserModel? user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        return user is not null && user.Value.GuildIds.Contains(request.GuildId);
    }
}