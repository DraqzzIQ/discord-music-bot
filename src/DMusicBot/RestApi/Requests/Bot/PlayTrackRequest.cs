using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class PlayTrackRequest : GuildRequest
{
    [FromBody] public LavalinkTrack[] Tracks { get; init; }
}