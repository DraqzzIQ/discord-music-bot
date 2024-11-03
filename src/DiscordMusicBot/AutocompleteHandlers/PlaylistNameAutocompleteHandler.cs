using Discord;
using Discord.Interactions;
using DiscordMusicBot.Attributes;
using DiscordMusicBot.Services;

namespace DiscordMusicBot.AutocompleteHandlers;

public class PlaylistNameAutocompleteHandler(IDbService dbService) : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = [];
        var onlyOwn = parameter.Attributes.Any(a => a is OnlyOwnAttribute);
        var ownAndPublic = parameter.Attributes.Any(a => a is OwnAndPublicAttribute);

        (await dbService
                .FindMatchingPlaylistsAsync(context.Guild.Id,
                    autocompleteInteraction.Data.Current.Value as string ?? "").ConfigureAwait(false))
            .Where(p => !onlyOwn || p.OwnerId == context.User.Id)
            .Where(p => !ownAndPublic || p.OwnerId == context.User.Id || p.IsPublic)
            .ToList()
            .ForEach(playlist => { results.Add(new AutocompleteResult(playlist.Name, playlist.Name)); });

        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}