using Discord.Interactions;
using Lavalink4NET;
using Microsoft.Extensions.Logging;
using Lavalink4NET.Integrations.LyricsJava.Extensions;

namespace DMusicBot.Modules;
public sealed class LyricsModule(IAudioService audioService, ILogger<LyricsModule> logger) : BaseModule(audioService, logger)
{
    /// <summary>
    ///     Shows lyrics to the song currently playing asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("lyrics", description: "Searches for lyrics", runMode: RunMode.Async)]
    public async Task Lyrics()
    {
        await DeferAsync().ConfigureAwait(false);

        var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

        if (player is null)
        {
            return;
        }

        var track = player.CurrentTrack;

        if (track is null)
        {
            await FollowupAsync("🤔 No track is currently playing.").ConfigureAwait(false);
            return;
        }

        var lyrics = await _audioService.Tracks.GetGeniusLyricsAsync(track.Title).ConfigureAwait(false);

        if (lyrics is null)
        {
            await FollowupAsync("😖 No lyrics found.").ConfigureAwait(false);
            return;
        }

        await FollowupAsync($"📃 Lyrics for {track.Title} by {track.Author}:\n{lyrics.Text}").ConfigureAwait(false);
    }
}