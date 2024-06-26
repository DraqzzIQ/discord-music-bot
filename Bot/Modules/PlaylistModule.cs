using Discord;
using Discord.Interactions;
using DMusicBot.AutocompleteHandlers;
using DMusicBot.Extensions;
using DMusicBot.Models;
using DMusicBot.Services;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;

[Group("playlist", "Playlist Management")]
public class PlaylistModule(IAudioService audioService, ILogger<PauseModule> logger, IDbService dbService)
    : BaseModule(audioService, logger)
{
    private readonly IDbService _dbService = dbService;
    private const int MaxPlaylistNameLength = 100;

    /// <summary>
    ///    Creates a new playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <param name="publicPlaylist">whether the playlist is public</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("create", "Create a new playlist", runMode: RunMode.Async)]
    public async Task CreateAsync([Summary("playlist_name", "Name of the playlist to create")] string name, [Summary("public_playlist", "Public means everyone can edit the playlist")] bool publicPlaylist = false)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name);
        if (playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** already exists.").ConfigureAwait(false);
            return;
        }
        
        if(name.Length > MaxPlaylistNameLength)
        {
            await FollowupAsync($"Playlist name is too long. Max length is **{MaxPlaylistNameLength}**.").ConfigureAwait(false);
            return;
        }
        
        PlaylistModel playlist = await _dbService.CreatePlaylistAsync(Context.User.Id,  Context.Guild.Id, name, publicPlaylist).ConfigureAwait(false);
        await FollowupAsync($"Created {(publicPlaylist ? "public" : "private")} playlist {playlist.Name}.").ConfigureAwait(false);
    }

    /// <summary>
    ///    Delete a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("delete", "delete a playlist", runMode: RunMode.Async)]
    public async Task DeleteAsync(
        [Summary("playlist_name", "Name of the playlist to delete"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if(!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{name}** is a private playlist and you are not the owner.").ConfigureAwait(false);
            return;
        }
        
        await _dbService.DeletePlaylistAsync(playlist).ConfigureAwait(false);
        
        await FollowupAsync($"Deleted playlist **{name}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///    List tracks in a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("list-tracks", "Lists all tracks in a playlist", runMode: RunMode.Async)]
    public async Task ListTracksAsync(
        [Summary("playlist_name", "Name of the playlist to list"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        List<TrackModel> tracks = playlist.Tracks;

        Embed[] embeds = EmbedCreator.CreateEmbeds($"Tracks in {playlist.Name}", tracks.Select(t => $"[{t.Title}]({t.Uri}) by {t.Author}").ToList());
        await FollowupAsync(embeds: embeds).ConfigureAwait(false);
    }

    /// <summary>
    ///    Shuffle a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("shuffle", "Shuffles a playlist", runMode: RunMode.Async)]
    public async Task ShuffleAsync(
        [Summary("playlist_name", "Name of the playlist to shuffle"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }
        
        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if(!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{name}** is a private playlist and you are not the owner.").ConfigureAwait(false);
            return;
        }
        playlist.Tracks.Shuffle();

        await _dbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);
        
        await FollowupAsync($"Shuffled **{playlist.Name}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///    Rename a playlist asynchronously.
    /// </summary>
    /// <param name="originalName">the name of the playlist</param>
    /// <param name="newName">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("rename", "Rename a playlist", runMode: RunMode.Async)]
    public async Task RenameAsync(
        [Summary("original_playlist_name", "Name of the playlist to rename"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string originalName,
        [Summary("new_playlist_name", "New name of the playlist"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string newName)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, originalName).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{originalName}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, originalName).ConfigureAwait(false);
        if(!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{originalName}** is a private playlist and you are not the owner.").ConfigureAwait(false);
            return;
        }
        
        if(newName.Length > MaxPlaylistNameLength)
        {
            await FollowupAsync($"Playlist name is too long. Max length is **{MaxPlaylistNameLength}**.").ConfigureAwait(false);
            return;
        }
        
        playlist.Name = newName;
        await _dbService.UpdatePlaylistAsync(playlist).ConfigureAwait(false);
        
        await FollowupAsync($"Renamed **{originalName}** to **{newName}**.").ConfigureAwait(false);
    }

    /// <summary>
    ///    Play a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", "Play a playlist", runMode: RunMode.Async)]
    public async Task PlayAsync(
        [Summary("playlist_name", "Name of the playlist to play"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        // Set the text channel for the event handler
        AudioServiceEventHandler.TextChannel = Context.Channel;
        
        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }
        
        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);

        List<LavalinkTrack> tracks = playlist.Tracks.Select(t => LavalinkTrack.Parse(t.SerializationString, null)).ToList();
        if (tracks.Count == 0)
        {
            await FollowupAsync($"😖 No tracks in the playlist **{name}**.").ConfigureAwait(false);
            return;
        }
        
        if (player.CurrentItem is null)
        {
            await player.PlayAsync(tracks[0]).ConfigureAwait(false);
            await player.Queue.InsertRangeAsync(0, tracks.Skip(1).Select(t => new TrackQueueItem(t))).ConfigureAwait(false);
        }
        else
        {
            await player.Queue.InsertRangeAsync(0, tracks.Select(t => new TrackQueueItem(t))).ConfigureAwait(false);
        }

        await FollowupAsync($"🔈 Added **{name}** to queue").ConfigureAwait(false);
    }

    /// <summary>
    ///    List all playlists asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("list", "Lists all playlists", runMode: RunMode.Async)]
    public async Task ListAsync()
    {
        await DeferAsync().ConfigureAwait(false);
        
        List<PlaylistModel> playlists = await _dbService.GetPlaylistsAsync(Context.Guild.Id);
        Embed[] embeds = EmbedCreator.CreateEmbeds("Playlists", playlists.Select(p => $"{p.Name} - {p.Tracks.Count}").ToList());
        await FollowupAsync(embeds: embeds);
    }

    /// <summary>
    ///    Add a track to a playlist asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("add-track", "Add a track to a playlist", runMode: RunMode.Async)]
    public async Task AddTrackAsync(
        [Summary("playlist_name", "Name of the playlist to add the track to"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name,
        [Summary("track", "The name or link to a track")] string query)
    {
        await DeferAsync().ConfigureAwait(false);

        var tracks = await _audioService.Tracks.LoadTracksAsync(query, TrackSearchMode.Deezer).ConfigureAwait(false);
        if (tracks.Count == 0)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        if (tracks.IsPlaylist)
        {
            await _dbService.AddTracksToPlaylistAsync(Context.Guild.Id, name, tracks.Tracks.Select(t => new TrackModel
            {
                Title = t.Title,
                Author = t.Author,
                Isrc = t.Isrc,
                Identifier = t.Identifier,
                SourceName = t.SourceName,
                Uri = t.Uri,
                ArtworkUri = t.ArtworkUri,
                Duration = t.Duration,
                SerializationString = t.ToString()
            }).ToList());

            await FollowupAsync($"🔈 Added {tracks.Count} tracks to **{name}**").ConfigureAwait(false);
            return;
        }


        // no playlist
        var track = tracks.Track;

        if (track is null)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }
        
        TrackModel trackModel = new()
        {
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

        await _dbService.AddTrackToPlaylistAsync(Context.Guild.Id, name, trackModel).ConfigureAwait(false);

        Embed embed = EmbedCreator.CreateEmbed($"Added to **{name}**", $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {track.Duration}", Color.Blue, true, track.ArtworkUri);
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }

    /// <summary>
    ///    Remove a track from a playlist asynchronously.
    /// </summary>
    /// <param name="playlistName">the name of the playlist</param>
    /// <param name="trackName">the name of the track</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("remove-track", "Remove a track from a playlist", runMode: RunMode.Async)]
    public async Task RemoveTrackAsync(
        [Summary("playlist_name", "Name of the playlist to remove the track from"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string playlistName,
        [Summary("track", "The name of the track to remove"), Autocomplete(typeof(PlaylistTrackAutocompleteHandler))] string trackName)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, playlistName).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{playlistName}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, playlistName).ConfigureAwait(false);
        if(!playlist.IsPublic && playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"**{playlistName}** is a private playlist and you are not the owner.").ConfigureAwait(false);
            return;
        }
        
        bool trackExists = await _dbService.TrackExistsInPlaylistAsync(Context.Guild.Id, playlistName, trackName).ConfigureAwait(false);
        if (!trackExists)
        {
            await FollowupAsync($"Track **{trackName}** does not exist in **{playlistName}**.").ConfigureAwait(false);
            return;
        }
        
        await _dbService.RemoveTrackFromPlaylistAsync(Context.Guild.Id, playlistName, trackName).ConfigureAwait(false);
        
        await FollowupAsync($"Removed **{trackName}** from **{playlistName}**.").ConfigureAwait(false);
    }
    
    /// <summary>
    ///    Set a playlist to public asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("set-public", "Set a playlist to public ", runMode: RunMode.Async)]
    public async Task SetPublicAsync(
        [Summary("playlist_name", "Name of the playlist to set to public"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if(playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{name}**.").ConfigureAwait(false);
            return;
        }
        
        await _dbService.SetPlaylistPublicAsync(Context.Guild.Id, name, true).ConfigureAwait(false);
        await FollowupAsync($"Set **{name}** to public.").ConfigureAwait(false);
    }
    
    /// <summary>
    ///    Set a playlist to private asynchronously.
    /// </summary>
    /// <param name="name">the name of the playlist</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("set-private", "Set a playlist to private", runMode: RunMode.Async)]
    public async Task SetPrivateAsync(
        [Summary("playlist_name", "Name of the playlist to set to private"),
         Autocomplete(typeof(PlaylistNameAutocompleteHandler))]
        string name)
    {
        await DeferAsync().ConfigureAwait(false);
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if (!playlistExists)
        {
            await FollowupAsync($"Playlist **{name}** does not exist.").ConfigureAwait(false);
            return;
        }

        PlaylistModel playlist = await _dbService.GetPlaylistAsync(Context.Guild.Id, name).ConfigureAwait(false);
        if(playlist.OwnerId != Context.User.Id)
        {
            await FollowupAsync($"You are not the owner of **{name}**.").ConfigureAwait(false);
            return;
        }
        
        await _dbService.SetPlaylistPublicAsync(Context.Guild.Id, name, false).ConfigureAwait(false);
        await FollowupAsync($"Set **{name}** to private.").ConfigureAwait(false);
    }
}