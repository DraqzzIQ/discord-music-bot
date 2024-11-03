using DiscordMusicBot.Dtos;
using DiscordMusicBot.Extensions;
using DiscordMusicBot.Models;
using DiscordMusicBot.RestApi.Requests;
using DiscordMusicBot.RestApi.Requests.Playlists;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class PlaylistsEndpointdefinition : BaseEndpointDefinition, IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/api/playlists/", GetPlaylistsAsync);
        app.MapGet("/api/playlists/playlist", GetPlaylist);
        app.MapGet("/api/playlists/add-track-previews", GetAddTrackToPlaylistPreviews);
        app.MapGet("/api/playlists/add-playlist-previews", GetAddPlaylistToPlaylistPreviews);

        app.MapPost("/api/playlists/pin", PinPlaylistAsync);
        app.MapPost("/api/playlists/unpin", UnpinPlaylistAsync);
        app.MapPost("/api/playlists/create", CreatePlaylistAsync);
        app.MapPost("/api/playlists/delete", DeletePlaylistAsync);
        app.MapPost("/api/playlists/edit", EditPlaylistAsync);
        app.MapPost("/api/playlists/shuffle", ShufflePlaylistAsync);
        app.MapPost("/api/playlists/delete-track", DeleteTrackAsync);
        app.MapPost("/api/playlists/add-track", AddTrackAsync);
        app.MapPost("/api/playlists/add-playlist", AddPlaylistToPlaylistAsync);
        app.MapPost("/api/playlists/reorder", ReorderPlaylistAsync);
    }

    public void DefineServices(IServiceCollection services)
    {
    }

    [Authorize]
    private async Task<IResult> GetPlaylistsAsync([AsParameters] GuildRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var playlists = await request.DbService.GetPlaylistsAsync(request.GuildId)
            .ConfigureAwait(false);

        var user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        if (user is null)
            return Results.NotFound("User not found");

        var userPropsTasks = playlists.Select(p => GetUserPropsAsync(p.OwnerId, request.DiscordSocketClient)).ToList();
        var userProps = await Task.WhenAll(userPropsTasks).ConfigureAwait(false);

        var playlistDtos = playlists.Select((p, index) => new UserPlaylistPreviewDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            OwnerUsername = userProps[index].username,
            OwnerAvatarUrl = userProps[index].avatarUrl,
            IsPublic = p.IsPublic,
            IsOwn = p.OwnerId == request.UserId,
            IsPinned = user.Value.PinnedPlaylists.Contains(p.Id),
            PreviewUrls = p.Tracks.Take(4).Select(s => s.ArtworkUri).ToList()
        }).ToList();

        return Results.Ok(playlistDtos);
    }

    [Authorize]
    private async Task<IResult> PinPlaylistAsync([AsParameters] PlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request).ConfigureAwait(false);
        if (!authorized)
            return result!;

        if (!user!.Value.PinnedPlaylists.Contains(playlist!.Id))
            user.Value.PinnedPlaylists.Add(playlist.Id);
        await request.DbService.UpdateUserAsync(user.Value).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> UnpinPlaylistAsync([AsParameters] PlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request).ConfigureAwait(false);
        if (!authorized)
            return result!;

        user!.Value.PinnedPlaylists.Remove(playlist!.Id);
        await request.DbService.UpdateUserAsync(user.Value).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> CreatePlaylistAsync([AsParameters] CreatePlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var trimmedName = request.Name.Trim();

        var error = await VerifyPlaylistName(trimmedName, request);
        if (error != "")
            return Results.BadRequest(error);

        PlaylistModel playlist = new()
        {
            GuildId = request.GuildId,
            OwnerId = request.UserId,
            Name = trimmedName,
            IsPublic = request.IsPublic,
            Tracks = []
        };

        await request.DbService.CreatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> DeletePlaylistAsync([AsParameters] PlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        await request.DbService.DeletePlaylistAsync(playlist!.Id).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> EditPlaylistAsync([AsParameters] EditPlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        var trimmedName = request.Name?.Trim() ?? playlist!.Name;

        var error = await VerifyPlaylistName(trimmedName, request, false);
        if (error != "")
            return Results.BadRequest(error);

        playlist!.Name = trimmedName;
        playlist.IsPublic = request.IsPublic ?? playlist.IsPublic;

        await request.DbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> ShufflePlaylistAsync([AsParameters] PlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, requireOwnerOrPublic: true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        playlist!.Tracks = playlist!.Tracks.Shuffle().ToList();
        await request.DbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> GetPlaylist([AsParameters] PlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request).ConfigureAwait(false);
        if (!authorized)
            return result!;

        var props = await GetUserPropsAsync(playlist!.OwnerId, request.DiscordSocketClient);

        UserPlaylistDto playlistDto = new()
        {
            Id = playlist!.Id.ToString(),
            Name = playlist.Name,
            OwnerUsername = props.username,
            OwnerAvatarUrl = props.avatarUrl,
            IsPublic = playlist.IsPublic,
            IsOwn = playlist.OwnerId == request.UserId,
            Tracks = playlist.Tracks.Select(t => new TrackDto
            {
                Id = t.Id.ToString(),
                Title = t.Title,
                Author = t.Author,
                DurationInSeconds = (int)t.Duration.TotalSeconds,
                ArtworkUrl = t.ArtworkUri,
                Url = t.Uri,
                EncodedTrack = t.SerializationString
            }).ToList()
        };

        return Results.Ok(playlistDto);
    }

    [Authorize]
    private async Task<IResult> DeleteTrackAsync([AsParameters] DeleteTrackRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, requireOwnerOrPublic: true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        if (request.TrackId is not null)
            playlist!.Tracks.RemoveAll(t => t.Id.ToString() == request.TrackId);
        else if (request.TrackUrl is not null)
            playlist!.Tracks.RemoveAll(t => t.Uri == request.TrackUrl);


        await request.DbService.UpdatePlaylistAsync(playlist!).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> AddTrackAsync([AsParameters] AddTrackRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, requireOwnerOrPublic: true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        if (request.Track.EncodedTrack is null)
            return Results.BadRequest("Encoded Track missing. Cannot add track to playlist");

        if (playlist!.Tracks.Any(t => t.Uri == request.Track.Url))
            return Results.BadRequest("Track already exists in playlist");

        TrackModel track = new()
        {
            Id = ObjectId.GenerateNewId(),
            Title = request.Track.Title,
            Author = request.Track.Author,
            Duration = TimeSpan.FromSeconds(request.Track.DurationInSeconds),
            ArtworkUri = request.Track.ArtworkUrl,
            Uri = request.Track.Url,
            SerializationString = request.Track.EncodedTrack
        };

        playlist!.Tracks.Add(track);
        await request.DbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> AddPlaylistToPlaylistAsync([AsParameters] AddPlaylistToPlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, requireOwnerOrPublic: true).ConfigureAwait(false);
        if (!authorized)
            return result!;


        TrackLoadResult? trackLoadResult = null;
        if (request.PlaylistUrl is not null)
            trackLoadResult = await request.AudioService.Tracks
                .LoadTracksAsync(request.PlaylistUrl, TrackSearchMode.None).ConfigureAwait(false);

        var tracks = request.PlaylistUrl is not null
            ? trackLoadResult!.Value.Tracks.ToArray()
            :
            [
                ..request.EncodedTracks!.Select(t =>
                    LavalinkTrack.Parse(t, null))
            ];

        // Dezeer albums artwork for tracks missing workaround
        if (request.PlaylistUrl is not null && tracks[0].SourceName == "deezer" &&
            tracks[0].ArtworkUri is null &&
            trackLoadResult!.Value.Playlist!.AdditionalInformation.ContainsKey("artworkUrl")
            && trackLoadResult!.Value.Playlist!.AdditionalInformation["artworkUrl"].GetString() is not null)
        {
            var artworkUri = new Uri(trackLoadResult!.Value.Playlist!.AdditionalInformation["artworkUrl"].GetString()!);

            tracks = tracks.Select(t => new LavalinkTrack
            {
                Title = t.Title,
                Identifier = t.Identifier,
                Author = t.Author,
                Duration = t.Duration,
                IsLiveStream = t.IsLiveStream,
                IsSeekable = t.IsSeekable,
                Uri = t.Uri,
                ArtworkUri = artworkUri,
                Isrc = t.Isrc,
                SourceName = t.SourceName,
                StartPosition = t.StartPosition,
                ProbeInfo = t.ProbeInfo,
                AdditionalInformation = t.AdditionalInformation
            }).ToArray();
        }

        var trackModels = tracks
            .Where(t => playlist!.Tracks.All(playlistTrack => playlistTrack.Uri != t.Uri))
            .Select(t => new TrackModel
            {
                Id = ObjectId.GenerateNewId(),
                Title = t.Title,
                Author = t.Author,
                Duration = t.Duration,
                ArtworkUri = t.ArtworkUri,
                Uri = t.Uri,
                SerializationString = t.ToString()
            }).ToList();

        playlist!.Tracks.AddRange(trackModels);
        await request.DbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> ReorderPlaylistAsync([AsParameters] ReorderPlaylistRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var (authorized, playlist, user, result) =
            await EnsureAccessToPlaylistAsync(request, requireOwnerOrPublic: true).ConfigureAwait(false);
        if (!authorized)
            return result!;

        var oldIndex = Math.Clamp(request.SourceIndex, 0, playlist!.Tracks.Count - 1);
        var newIndex = Math.Clamp(request.DestinationIndex, 0, playlist!.Tracks.Count - 1);
        if (oldIndex == newIndex)
            return Results.Ok();

        var track = playlist.Tracks[oldIndex];
        playlist.Tracks.RemoveAt(oldIndex);
        playlist.Tracks.Insert(newIndex, track);

        await request.DbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        return Results.Ok();
    }

    [Authorize]
    private async Task<IResult> GetAddTrackToPlaylistPreviews(
        [AsParameters] GetAddTrackToPlaylistPreviewRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var playlists = await request.DbService.GetPlaylistsAsync(request.GuildId)
            .ConfigureAwait(false);
        playlists = playlists.Where(p => p.OwnerId == request.UserId || p.IsPublic).ToList();

        var playlistDtos = playlists.Select(p => new AddItemToPlaylistDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            ContainsItem = p.Tracks.Any(t => t.Uri! == request.TrackUrl),
            PreviewUrls = p.Tracks.Take(4).Select(s => s.ArtworkUri).ToList()
        }).ToArray();

        return Results.Ok(playlistDtos);
    }

    [Authorize]
    private async Task<IResult> GetAddPlaylistToPlaylistPreviews(
        [AsParameters] GetAddPlaylistToPlaylistPreviewRequest request)
    {
        if (!await EnsureAuthorizedAsync(request))
            return Results.Unauthorized();

        var playlists = await request.DbService.GetPlaylistsAsync(request.GuildId)
            .ConfigureAwait(false);
        playlists = playlists.Where(p => p.OwnerId == request.UserId || p.IsPublic).ToList();

        TrackLoadResult? trackLoadResult = null;
        if (request.PlaylistUrl is not null)
            trackLoadResult = await request.AudioService.Tracks
                .LoadTracksAsync(request.PlaylistUrl, TrackSearchMode.None).ConfigureAwait(false);

        var tracks = request.PlaylistUrl is not null
            ? trackLoadResult!.Value.Tracks.ToArray()
            :
            [
                ..request.ParsedEncodedTracks!.Select(t =>
                    LavalinkTrack.Parse(t, null))
            ];

        var playlistDtos = playlists.Select(p => new AddItemToPlaylistDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            ContainsItem = tracks.All(lavaTrack => p.Tracks.Any(pt => pt.Uri! == lavaTrack.Uri)),
            PreviewUrls = p.Tracks.Take(4).Select(s => s.ArtworkUri).ToList()
        }).ToArray();

        return Results.Ok(playlistDtos);
    }

    private async Task<string> VerifyPlaylistName(string name, GuildRequest request, bool checkExists = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "A name is required";

        if (name.Length > 100)
            return "Name is too long";

        if (await request.DbService.PlaylistExistsAsync(request.GuildId, name).ConfigureAwait(false) && checkExists)
            return "A playlist with the same name already exists";

        return "";
    }
}