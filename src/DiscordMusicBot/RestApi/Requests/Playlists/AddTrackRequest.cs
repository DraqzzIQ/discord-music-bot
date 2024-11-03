using DiscordMusicBot.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DiscordMusicBot.RestApi.Requests.Playlists;

public class AddTrackRequest : PlaylistRequest
{
    [FromBody] public TrackDto Track { get; set; }
}