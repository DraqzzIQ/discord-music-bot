using Discord;
using Discord.Interactions;
using DiscordMusicBot.Services;

namespace DiscordMusicBot.AutocompleteHandlers;

public class PlaylistTrackAutocompleteHandler (IDbService dbService) : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
        IServiceProvider services)
    {
        List<AutocompleteResult> results = [];
        
        string playlistName = autocompleteInteraction.Data.Options.First(opt => opt.Name == "playlist_name").Value as string ?? "";
        if (string.IsNullOrWhiteSpace(playlistName))
        {
            return AutocompletionResult.FromSuccess();
        }
        
        bool playlistExists = await dbService.PlaylistExistsAsync(context.Guild.Id, playlistName).ConfigureAwait(false);
        if (!playlistExists)
        {
            return AutocompletionResult.FromSuccess();
        }
        
        (await dbService.FindMatchingTracksForPlaylistAsync(context.Guild.Id, playlistName,autocompleteInteraction.Data.Current.Value as string ?? "").ConfigureAwait(false))
            .ForEach(track => { results.Add(new AutocompleteResult(track.Title, track.Title)); });
        
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}