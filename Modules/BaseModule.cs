namespace DMusicBot.Modules;

using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET;
using Microsoft.Extensions.Logging;

/// <summary>
///     Presents some of the main features of the Lavalink4NET-Library.
/// </summary>
[RequireContext(ContextType.Guild)]
public class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly IAudioService _audioService;
    protected readonly ILogger<BaseModule> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseModule"/> class.
    /// </summary>
    /// <param name="audioService">the audio service</param>
    /// <param name="logger">the logger</param>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     thrown if the specified <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    public BaseModule(IAudioService audioService, ILogger<BaseModule> logger)
    {
        ArgumentNullException.ThrowIfNull(audioService);
        ArgumentNullException.ThrowIfNull(logger);

        _audioService = audioService;
        _logger = logger;
    }

    /// <summary>
    ///     Gets the guild player asynchronously.
    /// </summary>
    /// <param name="connectToVoiceChannel">
    ///     a value indicating whether to connect to a voice channel
    /// </param>
    /// <returns>
    ///     a task that represents the asynchronous operation. The task result is the lavalink player.
    /// </returns>
    protected async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await _audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Vote, retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await FollowupAsync(errorMessage).ConfigureAwait(false);
            return null;
        }

        return result.Player;
    }
}