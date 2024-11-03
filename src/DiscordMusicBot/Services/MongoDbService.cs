using DiscordMusicBot.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DiscordMusicBot.Services;

public class MongoDbService : IDbService
{
    private readonly IMongoCollection<BotChannelModel> _botChannelCollection;
    private readonly IMongoCollection<PlaylistModel> _playlistCollection;
    private readonly IMongoCollection<UserModel> _userCollection;

    public MongoDbService()
    {
        MongoClient client = new(ConfigService.DbConnectionString);
        var database = client.GetDatabase("music-bot");
        _playlistCollection = database.GetCollection<PlaylistModel>("playlists");
        _userCollection = database.GetCollection<UserModel>("users");
        _botChannelCollection = database.GetCollection<BotChannelModel>("bot-channels");
    }

    public async Task<bool> PlaylistExistsAsync(ulong guildId, string name)
    {
        return await _playlistCollection.Find(p => p.GuildId == guildId && p.Name == name).AnyAsync()
            .ConfigureAwait(false);
    }

    public async Task<bool> PlaylistExistsAsync(ObjectId id)
    {
        return await _playlistCollection.Find(p => p.Id == id).AnyAsync().ConfigureAwait(false);
    }

    public async Task<bool> TrackExistsInPlaylistAsync(ulong guildId, string playlistName, string trackName)
    {
        var playlist = await GetPlaylistAsync(guildId, playlistName).ConfigureAwait(false);
        return playlist.Tracks.Any(t => t.Title == trackName);
    }

    public async Task<PlaylistModel> GetPlaylistAsync(ulong guildId, string name)
    {
        return await _playlistCollection.Find(p => p.GuildId == guildId && p.Name == name).FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<PlaylistModel> GetPlaylistAsync(ObjectId id)
    {
        return await _playlistCollection.Find(p => p.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<PlaylistModel> CreatePlaylistAsync(ulong userId, ulong guildId, string name, string userName,
        bool publicPlaylist)
    {
        PlaylistModel playlist = new()
        {
            Name = name,
            OwnerId = userId,
            GuildId = guildId,
            IsPublic = publicPlaylist,
            Tracks = []
        };
        await _playlistCollection.InsertOneAsync(playlist).ConfigureAwait(false);
        return playlist;
    }

    public async Task CreatePlaylistAsync(PlaylistModel playlist)
    {
        await _playlistCollection.InsertOneAsync(playlist).ConfigureAwait(false);
    }

    public async Task DeletePlaylistAsync(PlaylistModel playlist)
    {
        await _playlistCollection.DeleteOneAsync(p => p.GuildId == playlist.GuildId && p.Name == playlist.Name)
            .ConfigureAwait(false);
    }

    public async Task DeletePlaylistAsync(ObjectId id)
    {
        await _playlistCollection.DeleteOneAsync(p => p.Id == id).ConfigureAwait(false);
    }

    public async Task AddTrackToPlaylistAsync(ulong guildId, string name, TrackModel track)
    {
        await _playlistCollection.UpdateOneAsync(p => p.GuildId == guildId && p.Name == name,
            Builders<PlaylistModel>.Update.Push(p => p.Tracks, track)).ConfigureAwait(false);
    }

    public async Task AddTracksToPlaylistAsync(ulong guildId, string name, IEnumerable<TrackModel> tracks)
    {
        await _playlistCollection.UpdateOneAsync(p => p.GuildId == guildId && p.Name == name,
            Builders<PlaylistModel>.Update.PushEach(p => p.Tracks, tracks)).ConfigureAwait(false);
    }

    public async Task RemoveTrackFromPlaylistAsync(ulong guildId, string playlistName, string trackName)
    {
        await _playlistCollection.UpdateOneAsync(p => p.GuildId == guildId && p.Name == playlistName,
            Builders<PlaylistModel>.Update.PullFilter(p => p.Tracks, s => s.Title == trackName)).ConfigureAwait(false);
    }

    public async Task<List<PlaylistModel>> GetPlaylistsAsync(ulong guildId)
    {
        return await _playlistCollection.Find(p => p.GuildId == guildId).ToListAsync().ConfigureAwait(false);
    }

    public async Task SetPlaylistPublicAsync(ulong guildId, string name, bool isPublic)
    {
        await _playlistCollection.UpdateOneAsync(p => p.GuildId == guildId && p.Name == name,
            Builders<PlaylistModel>.Update.Set(p => p.IsPublic, isPublic)).ConfigureAwait(false);
    }

    public async Task UpdatePlaylistAsync(PlaylistModel playlist)
    {
        await _playlistCollection.UpdateOneAsync(p => p.Id == playlist.Id, Builders<PlaylistModel>.Update
            .Set(p => p.Tracks, playlist.Tracks)
            .Set(p => p.IsPublic, playlist.IsPublic)
            .Set(p => p.Name, playlist.Name)).ConfigureAwait(false);
    }

    public async Task<List<PlaylistModel>> FindMatchingPlaylistsAsync(ulong guildId, string query)
    {
        return await _playlistCollection.Find(p => p.GuildId == guildId && p.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<TrackModel>> FindMatchingTracksForPlaylistAsync(ulong guildId, string playlistName,
        string query)
    {
        var playlist = await GetPlaylistAsync(guildId, playlistName).ConfigureAwait(false);
        return playlist.Tracks.Where(t => t.Title.ToLower().Contains(query.ToLower())).ToList();
    }

    public async Task UpdateUserAsync(UserModel userModel)
    {
        await _userCollection.UpdateOneAsync(a => a.UserId == userModel.UserId, Builders<UserModel>.Update
            .Set(a => a.PinnedPlaylists, userModel.PinnedPlaylists)
            .Set(a => a.Token, userModel.Token)
            .Set(a => a.GuildIds, userModel.GuildIds)).ConfigureAwait(false);
    }

    public async Task AddUserAsync(UserModel userModel)
    {
        await _userCollection.InsertOneAsync(userModel).ConfigureAwait(false);
    }

    public async Task<UserModel?> GetUserAsync(Guid token)
    {
        if (!await _userCollection.Find(a => a.Token == token).AnyAsync().ConfigureAwait(false))
            return null;

        return await _userCollection.Find(a => a.Token == token).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<UserModel?> GetUserAsync(ulong userId)
    {
        if (!await _userCollection.Find(a => a.UserId == userId).AnyAsync().ConfigureAwait(false))
            return null;

        return await _userCollection.Find(a => a.UserId == userId).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task SetBotChannelAsync(BotChannelModel botChannel)
    {
        await _botChannelCollection
            .ReplaceOneAsync(b => b.GuildId == botChannel.GuildId, botChannel, new ReplaceOptions { IsUpsert = true })
            .ConfigureAwait(false);
    }

    public async Task<BotChannelModel?> GetBotChannelAsync(ulong guildId)
    {
        if (!await _botChannelCollection.Find(b => b.GuildId == guildId).AnyAsync().ConfigureAwait(false))
            return null;

        return await _botChannelCollection.Find(b => b.GuildId == guildId).FirstOrDefaultAsync().ConfigureAwait(false);
    }
}