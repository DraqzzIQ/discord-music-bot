using DiscordMusicBot.Models;
using MongoDB.Bson;

namespace DiscordMusicBot.Services;

public interface IDbService
{
    public Task<bool> PlaylistExistsAsync(ulong guildId, string name);
    public Task<bool> PlaylistExistsAsync(ObjectId id);
    public Task<bool> TrackExistsInPlaylistAsync(ulong guildId, string playlistName, string trackName);
    public Task<PlaylistModel> GetPlaylistAsync(ulong guildId, string name);
    public Task<PlaylistModel> GetPlaylistAsync(ObjectId id);

    public Task<PlaylistModel> CreatePlaylistAsync(ulong userId, ulong guildId, string name, string userName,
        bool publicPlaylist);

    public Task CreatePlaylistAsync(PlaylistModel playlist);
    public Task DeletePlaylistAsync(PlaylistModel playlist);
    public Task DeletePlaylistAsync(ObjectId id);
    public Task AddTrackToPlaylistAsync(ulong guildId, string name, TrackModel track);
    public Task AddTracksToPlaylistAsync(ulong guildId, string name, IEnumerable<TrackModel> tracks);
    public Task RemoveTrackFromPlaylistAsync(ulong guildId, string playlistName, string trackName);
    public Task<List<PlaylistModel>> GetPlaylistsAsync(ulong guildId);
    public Task SetPlaylistPublicAsync(ulong guildId, string name, bool isPublic);
    public Task UpdatePlaylistAsync(PlaylistModel playlist);
    public Task<List<PlaylistModel>> FindMatchingPlaylistsAsync(ulong guildId, string query);
    public Task<List<TrackModel>> FindMatchingTracksForPlaylistAsync(ulong guildId, string playlistName, string query);
    public Task UpdateUserAsync(UserModel userModel);
    public Task AddUserAsync(UserModel userModel);
    public Task<UserModel?> GetUserAsync(Guid token);
    public Task<UserModel?> GetUserAsync(ulong userId);
    public Task SetBotChannelAsync(BotChannelModel botChannel);
    public Task<BotChannelModel?> GetBotChannelAsync(ulong guildId);
}