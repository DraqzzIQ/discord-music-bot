using Discord;
using Discord.WebSocket;
using DMusicBot.Models;
using DMusicBot.Services;
using DMusicBot.Util;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players.Queued;

namespace DMusicBot.AudioEventHandlers;

public class AudioServiceEventHandler(IDbService dbService, DiscordSocketClient discordSocketClient, IAudioService audioService)
{
    private readonly IDbService _dbService = dbService;
    private readonly DiscordSocketClient _discordSocketClient = discordSocketClient;
    private readonly IAudioService _audioService = audioService;

    private async Task TrackStartedHandler(object sender, TrackStartedEventArgs args)
    {
        ITextChannel? textChannel = await GuildChannelUtil.GetBotGuildChannel(_dbService, _discordSocketClient, args.Player.GuildId);
        
        if (textChannel is null)
            return;

        if (args.Player is QueuedLavalinkPlayer { RepeatMode: TrackRepeatMode.Track })
            return;

        Embed embed = EmbedCreator.CreateEmbed("Now Playing", $"[{args.Track.Title}]({args.Track.Uri})\n{args.Track.Author}\nDuration: {args.Track.Duration}",
            Color.Blue, true, args.Track.ArtworkUri);
        await textChannel.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }
    
    public void RegisterHandlers()
    {
        _audioService.TrackStarted += TrackStartedHandler;
    }
}