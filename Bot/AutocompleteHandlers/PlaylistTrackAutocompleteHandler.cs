using Discord;
using Discord.Interactions;
using DMusicBot.Models;
using DMusicBot.Services;

namespace DMusicBot.AutocompleteHandlers;

public class PlaylistTrackAutocompleteHandler (IDbService dbService) : AutocompleteHandler
{
    private readonly IDbService _dbService = dbService;
    
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
        IServiceProvider services)
    {
        List<AutocompleteResult> results = [];
        
        string playlistName = autocompleteInteraction.Data.Options.First(opt => opt.Name == "playlist_name").Value as string ?? "";
        if (string.IsNullOrWhiteSpace(playlistName))
        {
            return AutocompletionResult.FromSuccess();
        }
        
        bool playlistExists = await _dbService.PlaylistExistsAsync(context.Guild.Id, playlistName);
        if (!playlistExists)
        {
            return AutocompletionResult.FromSuccess();
        }
        
        (await _dbService.FindMatchingTracksForPlaylistAsync(context.Guild.Id, playlistName,autocompleteInteraction.Data.Current.Value as string ?? ""))
            .ForEach(track => { results.Add(new AutocompleteResult(track.Title, track.Title)); });
        
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}