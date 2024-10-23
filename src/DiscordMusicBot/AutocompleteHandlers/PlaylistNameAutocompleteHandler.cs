using Discord;
using Discord.Interactions;
using DiscordMusicBot.Services;

namespace DiscordMusicBot.AutocompleteHandlers;

public class PlaylistNameAutocompleteHandler(IDbService dbService) : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = [];

        (await dbService.FindMatchingPlaylistsAsync(context.Guild.Id, autocompleteInteraction.Data.Current.Value as string ?? "").ConfigureAwait(false))
            .ForEach(playlist => { results.Add(new AutocompleteResult(playlist.Name, playlist.Name)); });

        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}