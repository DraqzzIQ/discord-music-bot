using DiscordMusicBot.Dtos;
using Lavalink4NET.Players;

namespace DiscordMusicBot.SignalR.Clients;

public interface IBotClient
{
    Task UpdatePlayer(PlayerUpdatedDto dto);
    Task UpdatePosition(int position);
}