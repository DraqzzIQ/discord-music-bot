using Discord;
using Discord.Interactions;

namespace DiscordMusicBot.AutocompleteHandlers;

public class SearchModeAutoCompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
        IServiceProvider services)
    {
        var results = new List<AutocompleteResult>
        {
            new("Deezer", "Deezer"),
            new("Spotify", "Spotify"),
            new("YouTube", "YouTube"),
            new("YouTubeMusic", "YouTubeMusic"),
            new("Link", "Link")
        };
        
        results = results.Where(x => x.Name.Contains(autocompleteInteraction.Data.Current.Value as string ?? "", StringComparison.OrdinalIgnoreCase)).ToList();
        
        return Task.FromResult(AutocompletionResult.FromSuccess(results));
    }
}