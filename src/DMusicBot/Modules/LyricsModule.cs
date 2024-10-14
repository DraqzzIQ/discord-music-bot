using Discord;
using Discord.Interactions;
using DMusicBot.SignalR.Clients;
using DMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Lavalink4NET.Integrations.LyricsJava;
using Lavalink4NET.Integrations.LyricsJava.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DMusicBot.Modules;

public sealed class LyricsModule(IAudioService audioService, ILogger<LyricsModule> logger, IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    private const int MaxEmbedMessageSize = 4000;
    private const int MaxTotalMessageSize = 5900;

    /// <summary>
    ///     Shows lyrics to the track currently playing asynchronously.
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

        var lyrics = await _audioService.Tracks.GetCurrentTrackLyricsAsync(player).ConfigureAwait(false);

        if (lyrics is null)
        {
            await FollowupAsync("😖 No lyrics found.").ConfigureAwait(false);
            return;
        }

        Embed[] lyricsParts = SplitIntoChunks(lyrics, player.Position!.Value.Position).Take(10).ToArray();

        IUserMessage message = await FollowupAsync(embeds: lyricsParts).ConfigureAwait(false);

        // continue updating the message with the current lyrics
        // if (lyrics.TimedLines is null)
        //     return;
        //
        // LavalinkTrack currentTrack = player.CurrentTrack!;
        // TimedLyricsLine? currentLine = FindCurrentLine(lyrics, player.Position!.Value.Position);
        //
        // while (player.CurrentTrack is not null && player.CurrentTrack == currentTrack)
        // {
        //     TimedLyricsLine? nextLine = FindCurrentLine(lyrics, player.Position!.Value.Position);
        //     if (currentLine != nextLine && nextLine.HasValue && !string.IsNullOrEmpty(nextLine.Value.Line))
        //     {
        //         currentLine = nextLine;
        //         lyricsParts = SplitIntoChunks(lyrics, player.Position!.Value.Position).Take(10).ToArray();
        //
        //         for (int i = 0; i < lyricsParts.Length; i++)
        //             await message.ModifyAsync(m => m.Embeds = lyricsParts).ConfigureAwait(false);
        //     }
        //
        //     await Task.Delay(100).ConfigureAwait(false);
        // }
    }

    // private static TimedLyricsLine? FindCurrentLine(Lyrics lyrics, TimeSpan playerPosition)
    // {
    //     if (lyrics.TimedLines is null)
    //         return null;
    //
    //     return lyrics.TimedLines.Value.FirstOrDefault(line => line.Range.Start <= playerPosition && playerPosition <= line.Range.End);
    // }

    private static List<Embed> SplitIntoChunks(Lyrics lyrics, TimeSpan playerPosition)
    {
        List<Embed> chunks = [];
        EmbedBuilder chunk = new();
        bool reachedMaxSize = false;

        chunk.Title = $"📃 Lyrics for {lyrics.Track.Title} by {lyrics.Track.Author}:";

        // no timed lyrics
        if (lyrics.TimedLines is null || lyrics.TimedLines.Value.Length < 1)
        {
            List<string> lines = lyrics.Text.Split("\n").ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                chunk.Description += line + "\n";

                // max message length is 6000 characters, 1 embed = 4000 characters max
                int max = reachedMaxSize ? MaxTotalMessageSize - MaxEmbedMessageSize : MaxEmbedMessageSize;
                if (lines.Count <= i + 1 || chunk.Description.Length + lines[i + 1].Length + 10 <= max)
                    continue;

                chunks.Add(chunk.Build());
                chunk = new EmbedBuilder();
                // only 2 embeds with a combined max of 6000 characters are allowed, 1 embed = 4000 characters max
                if (reachedMaxSize)
                    break;
                reachedMaxSize = true;
            }

            if (!string.IsNullOrWhiteSpace(chunk.Description))
                chunks.Add(chunk.WithCurrentTimestamp().Build());

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

            // max message length is 6000 characters, 1 embed = 4000 characters max
            int max = reachedMaxSize ? MaxTotalMessageSize - MaxEmbedMessageSize : MaxEmbedMessageSize;
            if (lyrics.TimedLines.Value.Length <= i + 1 || chunk.Description.Length + lyrics.TimedLines.Value[i + 1].Line.Length + 10 <= max)
                continue;

            chunks.Add(chunk.Build());
            chunk = new EmbedBuilder();
            // only 2 embeds with a combined max of 6000 characters are allowed, 1 embed = 4000 characters max
            if (reachedMaxSize)
                break;
            reachedMaxSize = true;
        }

        if (!string.IsNullOrWhiteSpace(chunk.Description))
            chunks.Add(chunk.WithCurrentTimestamp().Build());

        return chunks;
    }
}