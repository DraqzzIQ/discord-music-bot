using Discord;
using Discord.Interactions;
using DMusicBot.Services;
using Lavalink4NET;

namespace DMusicBot.AutocompleteHandlers;

public class PlaylistNameAutocompleteHandler(IDbService dbService) : AutocompleteHandler
{
    private readonly IDbService _dbService = dbService;
    
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        IEnumerable<AutocompleteResult> results = new[]
        {
            new AutocompleteResult("playlist", "playlistname"),
        };
        
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}