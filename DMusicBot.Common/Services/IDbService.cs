using DMusicBot.Common.Models;

namespace DMusicBot.Common.Services;

public interface IDbService
{
    public Task<bool> PlaylistExistsAsync(ulong guildId, string name);
    public Task<bool> TrackExistsInPlaylistAsync(ulong guildId, string playlistName, string trackName);
    public Task<PlaylistModel> GetPlaylistAsync(ulong guildId, string name);
    public Task<PlaylistModel> CreatePlaylistAsync(ulong userId, ulong guildId, string name, bool publicPlaylist);
    public Task DeletePlaylistAsync(ulong guildId, string name);
    public Task DeletePlaylistAsync(PlaylistModel playlist);
    public Task AddTrackToPlaylistAsync(ulong guildId, string name, TrackModel track);
    public Task AddTracksToPlaylistAsync(ulong guildId, string name, IEnumerable<TrackModel> tracks);
    public Task RemoveTrackFromPlaylistAsync(ulong guildId, string playlistName, string trackName);
    public Task<List<PlaylistModel>> GetPlaylistsAsync(ulong guildId);
    public Task SetPlaylistPublicAsync(ulong guildId, string name, bool isPublic);
    public Task UpdatePlaylistAsync(PlaylistModel playlist);
    public Task<List<PlaylistModel>> FindMatchingPlaylistsAsync(ulong guildId, string query);
    public Task<List<TrackModel>> FindMatchingTracksForPlaylistAsync(ulong guildId, string playlistName, string query);

}