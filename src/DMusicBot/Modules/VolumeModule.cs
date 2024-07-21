// using Discord.Interactions;
// using Lavalink4NET;
// using Microsoft.Extensions.Logging;
//
// namespace DMusicBot.Modules;
// public sealed class VolumeModule(IAudioService audioService, ILogger<VolumeModule> logger) : BaseModule(audioService, logger)
// {
//     /// <summary>
//     ///     Updates the player volume asynchronously.
//     /// </summary>
//     /// <param name="volume">the volume (0 - 1000)</param>
//     /// <returns>a task that represents the asynchronous operation</returns>
//     [SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
//     public async Task Volume([Summary("volume", "The volume to set")] [MinValue(0), MaxValue(1000)] int volume = 100)
//     {
//         if (volume is > 1000 or < 0)
//         {
//             await RespondAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
//             return;
//         }
//
//         var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
//
//         if (player is null)
//         {
//             return;
//         }
//
//         await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
//         await RespondAsync($"Volume updated: {volume}%").ConfigureAwait(false);
//     }
// }