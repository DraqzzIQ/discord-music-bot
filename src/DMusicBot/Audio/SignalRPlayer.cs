using System.Collections.Immutable;
using DMusicBot.Dtos;
using DMusicBot.Extensions;
using DMusicBot.SignalR.Clients;
using DMusicBot.SignalR.Hubs;
using Lavalink4NET.Clients;
using Lavalink4NET.InactivityTracking.Players;
using Lavalink4NET.InactivityTracking.Trackers;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;
using Microsoft.AspNetCore.SignalR;

namespace DMusicBot.Audio;

public sealed class SignalRPlayer : QueuedLavalinkPlayer, IInactivityPlayerListener
{
    private readonly IHubContext<BotHub, IBotClient> _hubContext;

    public SignalRPlayer(IPlayerProperties<SignalRPlayer, SignalRPlayerOptions> properties) : base(properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        _hubContext = properties.Options.Value.HubContext;
        Task.Run(UpdateLoopAsync);
    }

    public async ValueTask<int> PlaySignalRAsync(LavalinkTrack track, bool enqueue = true, TrackPlayProperties properties = default,
        CancellationToken cancellationToken = default)
    {
        int index = await PlayAsync(track, enqueue, properties, cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync().ConfigureAwait(false);
        return index;
    }
    
    public async ValueTask AddRangeSignalRAsync(IEnumerable<LavalinkTrack> tracks, CancellationToken cancellationToken = default)
    {
        await Queue.AddRangeAsync(tracks.Select(t => new TrackQueueItem(t)).ToList(), cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
    }

    public async ValueTask SkipSignalRAsync(int count = 1, CancellationToken cancellationToken = default)
    {
        await SkipAsync(count, cancellationToken).ConfigureAwait(false);
        // set the position to 0 because of race conditions
        await UpdatePlayerAsync(updatedPositionInSeconds: 0, updateQueue: true).ConfigureAwait(false);
    }

    public async ValueTask SeekSignalRAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        await SeekAsync(position, cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updatedPositionInSeconds: (int)position.TotalSeconds).ConfigureAwait(false);
    }

    public async ValueTask PauseSignalRAsync(CancellationToken cancellationToken = default)
    {
        await PauseAsync(cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync().ConfigureAwait(false);
    }

    public async ValueTask ResumeSignalRAsync(CancellationToken cancellationToken = default)
    {
        await ResumeAsync(cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync().ConfigureAwait(false);
    }

    public async ValueTask StopSignalRAsync(CancellationToken cancellationToken = default)
    {
        await StopAsync(cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
    }
    
    public async ValueTask ClearQueueSignalRAsync(CancellationToken cancellationToken = default)
    {
        await Queue.ClearAsync(cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
    }
    
    public async ValueTask ShuffleQueueSignalRAsync(CancellationToken cancellationToken = default)
    {
        await Queue.ShuffleAsync(cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
    }
    
    public async ValueTask<bool> RemoveAtSignalRAsync(int index, CancellationToken cancellationToken = default)
    {
        if(index < 0 || index >= Queue.Count)
            return false;
        
        await Queue.RemoveAtAsync(index, cancellationToken).ConfigureAwait(false);
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
        return true;
    }

    public async ValueTask<bool> ReorderQueueSignalRAsync(int sourceIndex, int destinationIndex, CancellationToken cancellationToken = default)
    {
        if(sourceIndex == destinationIndex || sourceIndex < 0 || destinationIndex < 0 || sourceIndex >= Queue.Count || destinationIndex >= Queue.Count)
            return false;
        
        ITrackQueueItem item = Queue.ElementAt(sourceIndex);
        await Queue.RemoveAtAsync(sourceIndex, cancellationToken).ConfigureAwait(false);
        await Queue.InsertAsync(destinationIndex, item, cancellationToken).ConfigureAwait(false);
        
        await UpdatePlayerAsync(updateQueue: true).ConfigureAwait(false);
        return true;
    }

    protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
    {
        await base.NotifyTrackStartedAsync(track, cancellationToken);
        // set the position to 0 because of race conditions
        await UpdatePlayerAsync(updatedPositionInSeconds: 0, updateQueue: true).ConfigureAwait(false);
    }

    protected override async ValueTask NotifyTrackEndedAsync(ITrackQueueItem track, TrackEndReason reason, CancellationToken cancellationToken = default)
    {
        await base.NotifyTrackEndedAsync(track, reason, cancellationToken);
        // set the position to 0 because of race conditions
        await UpdatePlayerAsync(updatedPositionInSeconds: 0).ConfigureAwait(false);
    }

    private async ValueTask UpdatePlayerAsync(bool updateQueue = false, ITrackQueueItem? updatedTrack = null, int? updatedPositionInSeconds = null, ITrackQueue? updatedQueue = null, PlayerState? updatedState = null)
    {
        ITrackQueue queue = updatedQueue ?? Queue;
        ITrackQueueItem? currentItem = updatedTrack ?? CurrentItem;
        int positionInSeconds = updatedPositionInSeconds ?? (Position.HasValue ? (int)Position.Value.Position.TotalSeconds : 0);
        PlayerState state = updatedState ?? State;

        var dto = new PlayerUpdatedDto
        {
            UpdateQueue = updateQueue,
            CurrentTrack = currentItem?.ToTrackDto(),
            PositionInSeconds = positionInSeconds,
            Queue = updateQueue ? queue.Select(i => i.ToTrackDto()).ToArray() : [],
            State = state
        };
        
        await _hubContext.Clients.Group(GuildId.ToString()).UpdatePlayer(dto).ConfigureAwait(false);
    }
    
    // keep that bs in sync
    private async Task UpdateLoopAsync()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            if(State != PlayerState.Playing)
                continue;
            
            await _hubContext.Clients.Group(GuildId.ToString())
                .UpdatePosition(Position.HasValue ? (int)Position.Value.Position.TotalSeconds : 0)
                .ConfigureAwait(false);
        }
    }

    public ValueTask NotifyPlayerActiveAsync(PlayerTrackingState state, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return default;
    }

    public async ValueTask NotifyPlayerInactiveAsync(PlayerTrackingState state, CancellationToken cancellationToken = default)
    {
        // This method is called when the player reached the inactivity deadline.
        // For example: All users in the voice channel left and the player was inactive for longer than 30 seconds.
        cancellationToken.ThrowIfCancellationRequested();
        var dto = new PlayerUpdatedDto
        {
            UpdateQueue = true,
            CurrentTrack = null,
            PositionInSeconds = 0,
            Queue = [],
            State = PlayerState.Destroyed
        };
        
        await _hubContext.Clients.Group(GuildId.ToString()).UpdatePlayer(dto).ConfigureAwait(false);
    }

    public ValueTask NotifyPlayerTrackedAsync(PlayerTrackingState state, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return default;
    }
}