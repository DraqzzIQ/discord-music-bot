﻿using Discord.Interactions;
using DiscordMusicBot.SignalR.Clients;
using DiscordMusicBot.SignalR.Hubs;
using Lavalink4NET;
using Lavalink4NET.Players;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Modules;

public sealed class PauseModule(
    IAudioService audioService,
    ILogger<PauseModule> logger,
    IHubContext<BotHub, IBotClient> hubContext) : BaseModule(audioService, logger, hubContext)
{
    /// <summary>
    ///     Pauses the music asynchronously.
    /// </summary>
    /// <returns>a task that represents the asynchronous operation</returns>
    [SlashCommand("pause", "Pauses the player.", runMode: RunMode.Async)]
    public async Task PauseAsync()
    {
        var player = await GetPlayerAsync(false).ConfigureAwait(false);

        if (player is null) return;

        if (player.State is PlayerState.Paused)
        {
            await RespondAsync("Player is already paused.").ConfigureAwait(false);
            return;
        }

        await player.PauseSignalRAsync().ConfigureAwait(false);
        await RespondAsync("Paused.").ConfigureAwait(false);
    }
}