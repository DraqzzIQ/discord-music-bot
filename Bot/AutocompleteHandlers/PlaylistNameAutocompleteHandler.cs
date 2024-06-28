using Discord;
using Discord.Interactions;
using DMusicBot.Common.Services;

namespace DMusicBot.AutocompleteHandlers;

public class PlaylistNameAutocompleteHandler(IDbService dbService) : AutocompleteHandler
{
    private readonly IDbService _dbService = dbService;

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = [];

        (await _dbService.FindMatchingPlaylistsAsync(context.Guild.Id, autocompleteInteraction.Data.Current.Value as string ?? ""))
            .ForEach(playlist => { results.Add(new AutocompleteResult(playlist.Name, playlist.Name)); });

        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}