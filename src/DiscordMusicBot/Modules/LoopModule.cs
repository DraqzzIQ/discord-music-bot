﻿using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class LoopModule(
    IAudioService audioService,
    ILogger<LoopModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Loops the current track asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("loop", "Loops the current track.", runMode: RunMode.Async)]
    public async Task LoopAsync()
    {
        var player = await GetPlayerAsync(false).ConfigureAwait(false);

        if (player is null) return;

        if (player.RepeatMode != TrackRepeatMode.Track)
        {
            player.RepeatMode = TrackRepeatMode.Track;
            await RespondAsync("Looping enabled.").ConfigureAwait(false);
            return;
        }

        player.RepeatMode = TrackRepeatMode.None;
        await RespondAsync("Looping disabled.").ConfigureAwait(false);
    }
}