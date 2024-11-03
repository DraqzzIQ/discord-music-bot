using Discord;
using Discord.Interactions;
using DiscordMusicBot.Attributes;
using DiscordMusicBot.AutocompleteHandlers;
using DiscordMusicBot.Extensions;
using DiscordMusicBot.Models;
using DiscordMusicBot.Services;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace DiscordMusicBot.Modules;

[Group("playlist", "Playlist Management")]
public class PlaylistModule(
    IAudioService audioService,
    ILogger<PauseModule> logger,
    IDbService dbService,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    private const int MaxPlaylistNameLength = 100;

    /// <summary>
    ///     Creates a new playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <param name="publicPlaylist">whether the playlist is public</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("create", "Create a new playlist", runMode: RunMode.Async)]
    public async Task CreateAsync([Summary("playlist_name", "Name of the playlist to create")] string name,
        [Summary("public_playlist", "Public means everyone can edit the playlist")]
        bool publicPlaylist = false)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** already exists.").ConfigureAwait(false);
            return;
        }

        if (name.Length > MaxPlaylistNameLength)
        {
            await FollowupAsync($"Playlist name is too long. Max length is **{MaxPlaylistNameLength}**.")
                .ConfigureAwait(false);
            return;
        }

        var playlist = await dbService
            .CreatePlaylistAsync(Context.User.Id, Context.Guild.Id, name, Context.User.Username, publicPlaylist)
            .ConfigureAwait(false);
        await FollowupAsync($"Created {(publicPlaylist ? "public" : "private")} playlist **{playlist.Name}**.")
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Delete a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("delete", "delete a playlist", runMode: RunMode.Async)]
    public async Task DeleteAsync(
        [Summary("playlist_name", "Name of the playlist to delete")]
        [OnlyOwn]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{name}**.").ConfigureAwait(false);
            return;
        }

        await dbService.DeletePlaylistAsync(playlist).ConfigureAwait(false);

        await FollowupAsync($"Deleted playlist **{name}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///     List tracks in a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("list-tracks", "Lists all tracks in a playlist", runMode: RunMode.Async)]
    public async Task ListTracksAsync(
        [Summary("playlist_name", "Name of the playlist to list")]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        var tracks = playlist.Tracks;

        var embeds = EmbedCreator.CreateEmbeds($"Tracks in {playlist.Name}",
            tracks.Select(t => $"[{t.Title}]({t.Uri}) by {t.Author}").ToList());
        await FollowupAsync(embeds: embeds).ConfigureAwait(false);
    }

    /// <summary>
    ///     Shuffle a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("shuffle", "Shuffles a playlist", runMode: RunMode.Async)]
    public async Task ShuffleAsync(
        [Summary("playlist_name", "Name of the playlist to shuffle")]
        [OwnAndPublic]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{name}** is a private playlist and you are not the owner.").ConfigureAwait(false);
            return;
        }

        playlist.Tracks.Shuffle();

        await dbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        await FollowupAsync($"Shuffled **{playlist.Name}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Rename a playlist asynchronously.
    /// </summary>
    /// <param name="originalName">the name of the playlist</param>
    /// <param name="newName">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("rename", "Rename a playlist", runMode: RunMode.Async)]
    public async Task RenameAsync(
        [Summary("original_playlist_name", "Name of the playlist to rename")]
        [OnlyOwn]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string originalName,
        [Summary("new_playlist_name", "New name of the playlist")]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string newName)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, originalName).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{originalName}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, originalName).ConfigureAwait(false);
        if (!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{originalName}**.")
                .ConfigureAwait(false);
            return;
        }

        if (newName.Length > MaxPlaylistNameLength)
        {
            await FollowupAsync($"Playlist name is too long. Max length is **{MaxPlaylistNameLength}**.")
                .ConfigureAwait(false);
            return;
        }

        playlist.Name = newName;
        await dbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);

        await FollowupAsync($"Renamed **{originalName}** to **{newName}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Play a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", "Play a playlist", runMode: RunMode.Async)]
    public async Task PlayAsync(
        [Summary("playlist-name", "Name of the playlist to play")]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var player = await GetPlayerAsync().ConfigureAwait(false);
        if (player is null) return;

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);

        var tracks = playlist.Tracks.Select(t => LavalinkTrack.Parse(t.SerializationString, null))
            .ToList();
        if (tracks.Count == 0)
        {
            await FollowupAsync($"😖 No tracks in the playlist **{name}**.").ConfigureAwait(false);
            return;
        }

        if (player.CurrentItem is null)
        {
            await player.PlaySignalRAsync(tracks[0]).ConfigureAwait(false);
            await player.AddRangeSignalRAsync(tracks.Skip(1)).ConfigureAwait(false);
        }
        else
        {
            await player.AddRangeSignalRAsync(tracks).ConfigureAwait(false);
        }

        await FollowupAsync($"🔈 Added **{name}** to queue").ConfigureAwait(false);
    }

    /// <summary>
    ///     List all playlists asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("list", "Lists all playlists", runMode: RunMode.Async)]
    public async Task ListAsync()
    {
        await DeferAsync().ConfigureAwait(false);

        var playlists =
            await dbService.GetPlaylistsAsync(Context.Guild.Id).ConfigureAwait(false);
        var embeds =
            EmbedCreator.CreateEmbeds("Playlists", playlists.Select(p => $"{p.Name} - {p.Tracks.Count}").ToList());
        if (embeds.Length == 0)
        {
            await FollowupAsync("No playlists.").ConfigureAwait(false);
            return;
        }

        await FollowupAsync(embeds: embeds).ConfigureAwait(false);
    }

    /// <summary>
    ///     Add a track to a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("add-track", "Add a track to a playlist", runMode: RunMode.Async)]
    public async Task AddTrackAsync(
        [Summary("playlist_name", "Name of the playlist to add the track to")]
        [OwnAndPublic]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name,
        [Summary("track", "The name or link to a track")]
        string query,
        [Summary("source", "The source to search")] [Autocomplete(typeof(SearchModeAutoCompleteHandler))]
        string source = "Deezer")
    {
        await DeferAsync().ConfigureAwait(false);

        var searchMode = TrackSearchModeParser.Parse(source);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{name}** is a private playlist and you are not the owner.")
                .ConfigureAwait(false);
            return;
        }

        var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query, searchMode).ConfigureAwait(false);
        if (trackLoadResult.Count == 0)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        if (trackLoadResult.IsPlaylist)
        {
            var tracks = trackLoadResult.Tracks
                .ToArray();

            // Dezeer albums artwork for tracks missing workaround
            if (tracks[0].SourceName == "deezer" &&
                tracks[0].ArtworkUri is null && trackLoadResult.Playlist.AdditionalInformation.ContainsKey("artworkUrl")
                && trackLoadResult.Playlist.AdditionalInformation["artworkUrl"].GetString() is not null)
            {
                var artworkUri = new Uri(trackLoadResult.Playlist.AdditionalInformation["artworkUrl"].GetString()!);

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
                    ProbeInfo = t.ProbeInfo
                }).ToArray();
            }

            tracks = tracks.Where(t =>
                    playlist.Tracks.All(playlistTrack => playlistTrack.Uri != t.Uri))
                .ToArray();


            await dbService.AddTracksToPlaylistAsync(Context.Guild.Id, name, tracks.Select(t =>
                new TrackModel
                {
                    Id = ObjectId.GenerateNewId(),
                    Title = t.Title,
                    Author = t.Author,
                    Isrc = t.Isrc,
                    Identifier = t.Identifier,
                    SourceName = t.SourceName,
                    Uri = t.Uri,
                    ArtworkUri = t.ArtworkUri,
                    Duration = t.Duration,
                    SerializationString = t.ToString()
                }).ToList()).ConfigureAwait(false);

            await FollowupAsync($"🔈 Added {tracks.Length} tracks to **{name}**").ConfigureAwait(false);
            return;
        }


        // no playlist
        var track = trackLoadResult.Track;

        if (track is null)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        track = new LavalinkTrack
        {
            Title = track.Title,
            Identifier = track.Identifier,
            Author = track.Author,
            Duration = track.Duration,
            IsLiveStream = track.IsLiveStream,
            IsSeekable = track.IsSeekable,
            Uri = track.Uri,
            ArtworkUri = track.ArtworkUri,
            Isrc = track.Isrc,
            SourceName = track.SourceName,
            StartPosition = track.StartPosition,
            ProbeInfo = track.ProbeInfo
        };

        if (playlist.Tracks.Any(t => t.Uri == track.Uri))
        {
            await FollowupAsync($"Track **{track.Title}** already exists in **{name}**.").ConfigureAwait(false);
            return;
        }

        TrackModel trackModel = new()
        {
            Id = ObjectId.GenerateNewId(),
            Title = track.Title,
            Author = track.Author,
            Isrc = track.Isrc,
            Identifier = track.Identifier,
            SourceName = track.SourceName,
            Uri = track.Uri,
            ArtworkUri = track.ArtworkUri,
            Duration = track.Duration,
            SerializationString = track.ToString()
        };

        await dbService.AddTrackToPlaylistAsync(Context.Guild.Id, name, trackModel).ConfigureAwait(false);

        var embed = EmbedCreator.CreateEmbed($"Added to **{name}**",
            $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {TimeSpanFormatter.FormatDuration(track.Duration)}",
            Color.Blue, true, track.ArtworkUri);
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }

    /// <summary>
    ///     Remove a track from a playlist asynchronously.
    /// </summary>
    /// <param name="playlistName">the name of the playlist</param>
    /// <param name="trackName">the name of the track</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("remove-track", "Remove a track from a playlist", runMode: RunMode.Async)]
    public async Task RemoveTrackAsync(
        [Summary("playlist_name", "Name of the playlist to remove the track from")]
        [OwnAndPublic]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string playlistName,
        [Summary("track", "The name of the track to remove")] [Autocomplete(typeof(PlaylistTrackAutocompleteHandler))]
        string trackName)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, playlistName).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{playlistName}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, playlistName).ConfigureAwait(false);
        if (!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{playlistName}** is a private playlist and you are not the owner.")
                .ConfigureAwait(false);
            return;
        }

        var trackExists = await dbService.TrackExistsInPlaylistAsync(Context.Guild.Id, playlistName, trackName)
            .ConfigureAwait(false);
        if (!trackExists)
        {
            await FollowupAsync($"Track **{trackName}** does not exist in **{playlistName}**.").ConfigureAwait(false);
            return;
        }

        await dbService.RemoveTrackFromPlaylistAsync(Context.Guild.Id, playlistName, trackName).ConfigureAwait(false);

        await FollowupAsync($"Removed **{trackName}** from **{playlistName}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Set a playlist to public asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("set-public", "Set a playlist to public ", runMode: RunMode.Async)]
    public async Task SetPublicAsync(
        [Summary("playlist_name", "Name of the playlist to set to public")]
        [OnlyOwn]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{name}**.").ConfigureAwait(false);
            return;
        }

        await dbService.SetPlaylistPublicAsync(Context.Guild.Id, name, true).ConfigureAwait(false);
        await FollowupAsync($"Set **{name}** to public.").ConfigureAwait(false);
    }

    /// <summary>
    ///     Set a playlist to private asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("set-private", "Set a playlist to private", runMode: RunMode.Async)]
    public async Task SetPrivateAsync(
        [Summary("playlist_name", "Name of the playlist to set to private")]
        [OnlyOwn]
        [Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);

        var playlistExists = await dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        var playlist = await dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{name}**.").ConfigureAwait(false);
            return;
        }

        await dbService.SetPlaylistPublicAsync(Context.Guild.Id, name, false).ConfigureAwait(false);
        await FollowupAsync($"Set **{name}** to private.").ConfigureAwait(false);
    }
}