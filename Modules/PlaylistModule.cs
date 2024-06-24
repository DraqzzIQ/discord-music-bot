using Discord.Interactions;
using DMusicBot.AutocompleteHandlers;
using DMusicBot.Services;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;

[Group("playlist", "Playlist Management")]
public class PlaylistModule(IAudioService audioService, ILogger<PauseModule> logger, IDbService dbService) : BaseModule(audioService, logger)
{
    private readonly IDbService _dbService = dbService;

    [SlashCommand("create", "Create a new playlist", runMode: RunMode.Async)]
    public async Task CreateAsync([Summary("playlist_name", "Name of the playlist to create")] string name)
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("delete", "delete a playlist", runMode: RunMode.Async)]
    public async Task DeleteAsync([Summary("playlist_name", "Name of the playlist to delete"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string name)
    {
        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("list-songs", "Lists all tracks in a playlist", runMode: RunMode.Async)]
    public async Task ListSongsAsync([Summary("playlist_name", "Name of the playlist to list"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string name)
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("shuffle", "Shuffles a playlist", runMode: RunMode.Async)]
    public async Task ShuffleAsync([Summary("playlist_name", "Name of the playlist to shuffle"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string name)
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("rename", "Rename a playlist", runMode: RunMode.Async)]
    public async Task RenameAsync([Summary("original_playlist_name", "Name of the playlist to rename"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string originalName,
        [Summary("new_playlist_name", "New name of the playlist"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string newName)
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("play", "Play a playlist", runMode: RunMode.Async)]
    public async Task PlayAsync([Summary("playlist_name", "Name of the playlist to play"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string name)
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }
    
    [SlashCommand("list", "Lists all playlists", runMode: RunMode.Async)]
    public async Task ListAsync()
    {

        // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
        // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
    }

    [Group("Track", "Add or remove tracks from a playlist")]
    public class Track(IAudioService audioService, ILogger<PauseModule> logger, IDbService dbService) : BaseModule(audioService, logger)
    {
        private readonly IDbService _dbService = dbService;
        
        [SlashCommand("add", "Add a track to a playlist", runMode: RunMode.Async)]
        public async Task AddAsync([Summary("playlist_name", "Name of the playlist to add to"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string name,
            [Summary("track", "The name or link to a track")] string query)
        {

            // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
            // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
        }
        
        [SlashCommand("remove", "Remove a track from a playlist", runMode: RunMode.Async)]
        public async Task RemoveAsync([Summary("playlist_name", "Name of the playlist to remove from"), Autocomplete(typeof(PlaylistNameAutocompleteHandler))] string playlistName,
            [Summary("track", "The name of the track to remove")] string songName)
        {

            // var playlist = await _dbService.CreatePlaylistAsync(Context.User.Id, name);
            // await RespondAsync($"Created playlist {playlist.Name} with ID {playlist.Id}.");
        }
    }
}