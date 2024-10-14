using Microsoft.AspNetCore.Mvc;

namespace DMusicBot.RestApi.Requests.Bot;

public class SearchRequest : GuildRequest
{
    [FromQuery] public string Query { get; init; }
    [FromQuery] public string SearchMode { get; init; }
}