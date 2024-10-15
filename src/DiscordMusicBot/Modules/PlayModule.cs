using Discord;
using Discord.Interactions;
using DiscordMusicBot.AutocompleteHandlers;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;
public sealed class PlayModule(IAudioService audioService, ILogger<PlayModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Plays music asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play([Summary("track", "The name or link to a track")] string query,
        [Summary("source", "The source to search"), Autocomplete(typeof(SearchModeAutoCompleteHandler))] string source = "Deezer")
    {
        await DeferAsync().ConfigureAwait(false);
        
        TrackSearchMode searchMode = TrackSearchModeParser.Parse(source);

        var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }

        var tracks = await _audioService.Tracks.LoadTracksAsync(query, searchMode).ConfigureAwait(false);


        if (tracks.Count == 0)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        if (tracks.IsPlaylist)
        {
            if (player.CurrentItem is null)
            {
                await player.PlaySignalRAsync(tracks.Tracks[0]).ConfigureAwait(false);
                await player.AddRangeSignalRAsync(tracks.Tracks.Skip(1)).ConfigureAwait(false);
            }
            else
            {
                await player.AddRangeSignalRAsync(tracks.Tracks).ConfigureAwait(false);
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

        await player.PlaySignalRAsync(track).ConfigureAwait(false);

        Embed embed = EmbedCreator.CreateEmbed("Added to queue", $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {TimeSpanFormatter.FormatDuration(track.Duration)}", Color.Blue, true, track.ArtworkUri);
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }
}