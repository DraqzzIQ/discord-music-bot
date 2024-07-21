using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace DMusicBot.Audio;

public class SignalRPlayer(IPlayerProperties<SignalRPlayer, SignalRPlayerOptions> properties)
    : QueuedLavalinkPlayer(properties), ILavalinkPlayerListener
{
    
}