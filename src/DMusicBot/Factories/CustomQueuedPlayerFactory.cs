using DMusicBot.Audio;
using Lavalink4NET.Players;

namespace DMusicBot.Factories;

public static class CustomQueuedPlayerFactory
{
    public static PlayerFactory<SignalRPlayer, SignalRPlayerOptions> CustomQueued { get; } =
        CreatePlayerAsync;
    public static ValueTask<SignalRPlayer> CreatePlayerAsync(IPlayerProperties<SignalRPlayer, SignalRPlayerOptions> properties, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        return ValueTask.FromResult(new SignalRPlayer(properties));
    }
}