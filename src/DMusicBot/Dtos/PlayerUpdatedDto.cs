using Lavalink4NET.Players;

namespace DMusicBot.Dtos;

public record struct PlayerUpdatedDto
{
    public bool UpdateQueue { get; init; }
    public TrackDto? CurrentTrack { get; init; }
    public int PositionInSeconds { get; init; }
    public TrackDto[] Queue { get; init; }
    public PlayerState State { get; init; }
}