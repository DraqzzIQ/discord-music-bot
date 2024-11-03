using DiscordMusicBot.Dtos;

namespace DiscordMusicBot.SignalR.Clients;

public interface IBotClient
{
    Task UpdatePlayer(PlayerUpdatedDto dto);
    Task UpdatePosition(int position);
}