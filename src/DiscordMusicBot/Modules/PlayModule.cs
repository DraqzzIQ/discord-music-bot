using Discord;
using Discord.Interactions;
using DiscordMusicBot.AutocompleteHandlers;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class PlayModule(
    IAudioService audioService,
    ILogger<PlayModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Plays music asynchronously.
    /// </summary>
    /// <param name="query">the search query</param>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("play", "Plays music", runMode: RunMode.Async)]
    public async Task Play([Summary("track", "The name or link to a track")] string query,
        [Summary("source", "The source to search")] [Autocomplete(typeof(SearchModeAutoCompleteHandler))]
        string source = "Deezer")
    {
        await DeferAsync().ConfigureAwait(false);

        var searchMode = TrackSearchModeParser.Parse(source);

        var player = await GetPlayerAsync().ConfigureAwait(false);
        if (player is null) return;

        var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query, searchMode).ConfigureAwait(false);


        if (trackLoadResult.Count == 0)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        if (trackLoadResult.IsPlaylist)
        {
            var tracks = trackLoadResult.Tracks.ToArray();
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
                    ProbeInfo = t.ProbeInfo,
                    AdditionalInformation = t.AdditionalInformation
                }).ToArray();
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

            await FollowupAsync($"🔈 Added {trackLoadResult.Count} tracks to queue").ConfigureAwait(false);
            return;
        }


        // no playlist
        var track = trackLoadResult.Track;

        if (track is null)
        {
            await FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        await player.PlaySignalRAsync(track).ConfigureAwait(false);

        var embed = EmbedCreator.CreateEmbed("Added to queue",
            $"[{track.Title}]({track.Uri})\n{track.Author}\nDuration: {TimeSpanFormatter.FormatDuration(track.Duration)}",
            Color.Blue, true, track.ArtworkUri);
        await FollowupAsync(embed: embed).ConfigureAwait(false);
    }
}