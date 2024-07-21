using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class PlayTrackRequest : BaseRequest
{
    [FromBody] public LavalinkTrack[] Tracks { get; init; }
}