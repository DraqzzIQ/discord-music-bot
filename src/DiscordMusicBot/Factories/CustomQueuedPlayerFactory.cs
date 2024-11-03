using DiscordMusicBot.Audio;
using Lavalink4NET.Players;

namespace DiscordMusicBot.Factories;

public static class CustomQueuedPlayerFactory
{
    public static PlayerFactory<SignalRPlayer, SignalRPlayerOptions> CustomQueued { get; } =
        CreatePlayerAsync;

    private static ValueTask<SignalRPlayer> CreatePlayerAsync(
        IPlayerProperties<SignalRPlayer, SignalRPlayerOptions> properties, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        return ValueTask.FromResult(new SignalRPlayer(properties));
    }
}