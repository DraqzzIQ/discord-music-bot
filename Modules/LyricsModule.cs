using Discord;
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Integrations.LyricsJava;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;
public sealed class LyricsModule(IAudioService audioService, ILogger<LyricsModule> logger) : BaseModule(audioService, logger)
{
    private const int MaxMessageSize = 4000;

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

        var lyrics = await audioService.Tracks.GetCurrentTrackLyricsAsync(player).ConfigureAwait(false);

        if (lyrics is null)
        {
            await FollowupAsync("😖 No lyrics found.").ConfigureAwait(false);
            return;
        }

        Embed[] lyricsParts = SplitIntoChunks(lyrics, player.Position!.Value.Position).Take(10).ToArray();

        await FollowupAsync(embeds: lyricsParts).ConfigureAwait(false);
    }

    private static List<Embed> SplitIntoChunks(Lyrics lyrics, TimeSpan playerPosition)
    {
        List<Embed> chunks = new();
        EmbedBuilder chunk = new();

        chunk.Title = $"📃 Lyrics for {lyrics.Track.Title} by {lyrics.Track.Author}:";

        // no timed lyrics
        if (lyrics.TimedLines is null || lyrics.TimedLines.Value.Length < 1)
        {
            List<string> lines = lyrics.Text.Split("\n").ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                chunk.Description += line + "\n";

                if (lines.Count > i + 1 && chunk.Description.Length + lines[i + 1].Length + 10 > MaxMessageSize)
                {
                    chunks.Add(chunk.Build());
                    chunk = new EmbedBuilder();
                }
            }

            if (!string.IsNullOrWhiteSpace(chunk.Description))
                chunks.Add(chunk.Build());

            return chunks;
        }

        // timed lyrics
        for (int i = 0; i < lyrics.TimedLines.Value.Length; i++)
        {
            TimedLyricsLine line = lyrics.TimedLines.Value[i];

            // highlight the current line
            if (line.Range.Start <= playerPosition && playerPosition <= line.Range.End)
                chunk.Description += $"__**{line.Line}**__\n";
            else
                chunk.Description += line.Line + "\n";

            if (lyrics.TimedLines.Value.Length > i + 1 && chunk.Description.Length + lyrics.TimedLines.Value[i + 1].Line.Length + 10 > MaxMessageSize)
            {
                chunks.Add(chunk.Build());
                chunk = new EmbedBuilder();
            }
        }

        if (!string.IsNullOrWhiteSpace(chunk.Description))
            chunks.Add(chunk.Build());

        return chunks;
    }
}