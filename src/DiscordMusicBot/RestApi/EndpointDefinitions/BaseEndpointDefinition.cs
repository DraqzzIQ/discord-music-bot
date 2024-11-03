using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Models;
using DiscordMusicBot.RestApi.Requests;
using DiscordMusicBot.Services;
using DiscordMusicBot.Util;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace DiscordMusicBot.RestApi.EndpointDefinitions;

public class BaseEndpointDefinition
{
    private readonly Dictionary<ulong, (DateTime cachedAt, (string username, string avatarUrl) userProps)> _userCache =
        new();

    protected async Task SendMessageWithUserPrefixAsync(string message, ulong guildId, ulong userId,
        IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        var user = discordSocketClient.GetUser(userId);
        var channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId)
            .ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", allowedMentions: AllowedMentions.None)
            .ConfigureAwait(false);
    }

    protected async Task SendEmbedMessageWithUserPrefixAsync(string message, Embed embed, ulong guildId, ulong userId,
        IDbService dbService,
        DiscordSocketClient discordSocketClient)
    {
        var user = discordSocketClient.GetUser(userId);
        var channel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, guildId)
            .ConfigureAwait(false);
        if (channel is null)
            return;

        await channel.SendMessageAsync($"<@{user.Id}> {message}.", embed: embed, allowedMentions: AllowedMentions.None)
            .ConfigureAwait(false);
    }

    protected async Task<bool> EnsureAuthorizedAsync(GuildRequest request)
    {
        var user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        return user is not null && user.Value.GuildIds.Contains(request.GuildId);
    }


    protected async Task<(string username, string avatarUrl)> GetUserPropsAsync(ulong userId,
        DiscordSocketClient discordSocketClient)
    {
        if (_userCache.TryGetValue(userId, out var cached) &&
            DateTime.UtcNow - cached.cachedAt < TimeSpan.FromMinutes(30))
            return cached.userProps;

        var discordUser = await discordSocketClient.GetUserAsync(userId).ConfigureAwait(false);
        if (discordUser is null)
            return ("null", string.Empty);

        (string username, string avatarUrl) userProps = (discordUser.Username, discordUser.GetDisplayAvatarUrl());

        _userCache[userId] = (DateTime.UtcNow, userProps);

        return userProps;
    }

    protected async Task<(bool authorized, PlaylistModel? playlist, UserModel? user, IResult? result)>
        EnsureAccessToPlaylistAsync(
            PlaylistRequest request, bool requireOwner = false, bool requireOwnerOrPublic = false)
    {
        var user = await request.DbService.GetUserAsync(request.UserId).ConfigureAwait(false);
        if (user is null)
            return (false, null, null, Results.NotFound("User not found"));

        if (!ObjectId.TryParse(request.PlaylistId, out var id))
            return (false, null, null, Results.BadRequest("Invalid playlist id"));

        if (!await request.DbService.PlaylistExistsAsync(id).ConfigureAwait(false))
            return (false, null, null, Results.NotFound("Playlist not found"));

        var playlist = await request.DbService.GetPlaylistAsync(id).ConfigureAwait(false);
        if (!user.Value.GuildIds.Contains(playlist.GuildId))
            return (false, null, null, Results.NotFound("Playlist not found"));

        if (requireOwner && playlist.OwnerId != user.Value.UserId)
            return (false, null, null, Results.BadRequest("You are not the owner of this playlist"));

        if (requireOwnerOrPublic && playlist.OwnerId != user.Value.UserId && !playlist.IsPublic)
            return (false, null, null, Results.BadRequest("Playlist is not public"));

        return (true, playlist, user, null);
    }
}