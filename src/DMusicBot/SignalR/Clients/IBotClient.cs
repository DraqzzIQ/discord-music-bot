using DMusicBot.Dtos;
using Lavalink4NET.Players;

namespace DMusicBot.SignalR.Clients;

public interface IBotClient
{
    Task UpdatePlayer(PlayerUpdatedDto dto);
    Task UpdatePosition(int position);
}