using Discord;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players.Queued;

namespace DMusicBot;
internal static class AudioServiceEventHandler
{
    public static IMessageChannel? TextChannel = null;

    private static async Task TrackStartedHandler(object sender, TrackStartedEventArgs args)
    {
        if (TextChannel is null)
            return;

        if (args.Player is QueuedLavalinkPlayer player)
        {
            if (player.RepeatMode == TrackRepeatMode.Track)
                return;
        }

        Embed embed = EmbedCreator.CreateEmbed("Now Playing", $"[{args.Track.Title}]({args.Track.Uri})\n{args.Track.Author}\nDuration: {args.Track.Duration}", Color.Blue, true, args.Track.ArtworkUri);
        await TextChannel.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    public static void RegisterHandlers(IAudioService audioService)
    {
        ArgumentNullException.ThrowIfNull(audioService);

        audioService.TrackStarted += TrackStartedHandler;
    }
}