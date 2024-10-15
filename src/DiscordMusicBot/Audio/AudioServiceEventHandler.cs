using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Services;
using DiscordMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players.Queued;

namespace DiscordMusicBot.Audio;

public class AudioServiceEventHandler(IDbService dbService, DiscordSocketClient discordSocketClient, IAudioService audioService)
{
    private async Task TrackStartedHandler(object sender, TrackStartedEventArgs args)
    {
        ITextChannel? textChannel = await GuildChannelUtil.GetBotGuildChannel(dbService, discordSocketClient, args.Player.GuildId)
            .ConfigureAwait(false);
        
        if (textChannel is null)
            return;

        if (args.Player is SignalRPlayer { RepeatMode: TrackRepeatMode.Track })
            return;

        Embed embed = EmbedCreator.CreateEmbed("Now Playing", $"[{args.Track.Title}]({args.Track.Uri})\n{args.Track.Author}\nDuration: {TimeSpanFormatter.FormatDuration(args.Track.Duration)}",
            Color.Blue, true, args.Track.ArtworkUri);
        await textChannel.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }
    
    public void RegisterHandlers()
    {
        audioService.TrackStarted += TrackStartedHandler;
    }
}