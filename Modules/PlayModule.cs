using Discord;
using Discord.Interactions;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;
public sealed class PlayModule(IAudioService audioService, ILogger<PlayModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Plays music asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        await DeferAsync().ConfigureAwait(false);

        // Set the text channel for the event handler
        AudioServiceEventHandler.TextChannel = Context.Channel;

        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        var tracks = await _audioService.Tracks.LoadTracksAsync(query, TrackSearchMode.YouTube).ConfigureAwait(false);


        if (tracks.Count is 0)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        if (tracks.IsPlaylist)
        {
            if (player.CurrentItem is null)
            {
                await player.PlayAsync(tracks.Tracks[0]).ConfigureAwait(false);
                await player.Queue.InsertRangeAsync(0, tracks.Tracks.Skip(1).Select(t => new TrackQueueItem(t))).ConfigureAwait(false);
            }
            else
            {
                await player.Queue.InsertRangeAsync(0, tracks.Tracks.Select(t => new TrackQueueItem(t))).ConfigureAwait(false);
            }

            await FollowupAsync($"🔈 Added {tracks.Count} tracks to queue").ConfigureAwait(false);
            return;
        }


        // no playlist
        var track = tracks.Track;

        if (track is null)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        var position = await player.PlayAsync(track).ConfigureAwait(false);

        Embed embed = EmbedCreator.CreateEmbed("Added to queue", $"[{track.Title}]({track.Uri})\nDuration: {track.Duration}", Color.Blue, true, track.ArtworkUri);
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }
}