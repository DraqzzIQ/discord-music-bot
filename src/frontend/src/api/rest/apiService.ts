import {GuildDto} from "@/dtos/GuildDto";
import {SearchResponseDto} from "@/dtos/SearchResponseDto";
import {PlayRequestDto} from "@/dtos/PlayRequestDto";
import {TrackDto} from "@/dtos/TrackDto";

// make API requests with cookie authentication
async function apiRequest(url: string, options: RequestInit = {}, parse: boolean = true): Promise<any> {
    const response = await fetch(process.env.NEXT_PUBLIC_REST_API_URL + url, {
        credentials: 'include',
        ...options,
    });

    const text = await response.text();

    // log if debug
    if (!response.ok && process.env.NODE_ENV === 'development') {
        console.log(`API request failed with status ${response.status} and body ${text} for URL ${url}`);
    }

    if (!parse) {
        return text;
    }

    return text ? JSON.parse(text) : null;
}

export async function RequestGuilds(): Promise<GuildDto[]> {
    const data = await apiRequest('api/discord/guilds');

    return data.guilds.map((guild: any): GuildDto => ({
        id: guild.id,
        name: guild.name,
        iconUrl: guild.iconUrl,
    }));
}

export async function RequestRewind(guildId: number): Promise<void> {
    await apiRequest(`api/player/rewind?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestSkip(guildId: number, index: number = 0): Promise<void> {
    await apiRequest(`api/player/skip?GuildId=${guildId}&Index=${index}`, {method: 'POST'});
}

export async function RequestPause(guildId: number): Promise<void> {
    await apiRequest(`api/player/pause?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestResume(guildId: number): Promise<void> {
    await apiRequest(`api/player/resume?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestSeek(guildId: number, position: number): Promise<void> {
    await apiRequest(`api/player/position?GuildId=${guildId}&positionInSeconds=${position}`, {method: 'POST'});
}

export async function RequestShuffle(guildId: number): Promise<void> {
    await apiRequest(`api/player/shuffle?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestClear(guildId: number): Promise<void> {
    await apiRequest(`api/player/clear?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestDeduplicate(guildId: number): Promise<void> {
    await apiRequest(`api/player/deduplicate?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestRemove(guildId: number, index: number): Promise<void> {
    await apiRequest(`api/player/remove?GuildId=${guildId}&index=${index}`, {method: 'POST'});
}

export async function RequestReorder(guildId: number, sourceIndex: number, destinationIndex: number): Promise<void> {
    await apiRequest(`api/player/reorder?GuildId=${guildId}&sourceIndex=${sourceIndex}&destinationIndex=${destinationIndex}`, {method: 'POST'});
}

export async function RequestSearch(guildId: number, query: string, searchMode: string): Promise<SearchResponseDto | null> {
    query = encodeURIComponent(query);
    return await apiRequest(`api/player/search?GuildId=${guildId}&query=${query}&searchMode=${searchMode}`, {method: 'GET'});
}

export async function RequestPlay(guildId: number, playRequest: PlayRequestDto): Promise<boolean> {
    let response = await apiRequest(`api/player/play?GuildId=${guildId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(playRequest),
    }, false);

    return response === "";
}

export async function RequestLeave(guildId: number): Promise<void> {
    await apiRequest(`api/player/leave?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestStop(guildId: number): Promise<void> {
    await apiRequest(`api/player/stop?GuildId=${guildId}`, {method: 'POST'});
}

export async function RequestLyrics(guildId: number): Promise<any> {
    return await apiRequest(`api/player/lyrics?GuildId=${guildId}`, {method: 'GET'});
}


export async function RequestPlaylists(guildId: number): Promise<any> {
    return await apiRequest(`api/playlists?GuildId=${guildId}`, {method: 'GET'});
}

export async function RequestPinPlaylist(guildId: number, playlistId: string): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    await apiRequest(`api/playlists/pin?GuildId=${guildId}&PlaylistId=${playlistId}`, {method: 'POST'});
}

export async function RequestUnpinPlaylist(guildId: number, playlistId: string): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    await apiRequest(`api/playlists/unpin?GuildId=${guildId}&PlaylistId=${playlistId}`, {method: 'POST'});
}

export async function RequestCreatePlaylist(guildId: number, name: string, isPublic: boolean): Promise<any> {
    name = encodeURIComponent(name);
    return await apiRequest(`api/playlists/create?GuildId=${guildId}&Name=${name}&IsPublic=${isPublic}`, {method: 'POST'}, false);
}

export async function RequestDeletePlaylist(guildId: number, playlistId: string): Promise<any> {
    playlistId = encodeURIComponent(playlistId);
    return await apiRequest(`api/playlists/delete?GuildId=${guildId}&PlaylistId=${playlistId}`, {method: 'POST'}, false);
}

export async function RequestEditPlaylist(guildId: number, playlistId: string, name: string, isPublic: boolean): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    name = encodeURIComponent(name);
    return await apiRequest(`api/playlists/edit?GuildId=${guildId}&PlaylistId=${playlistId}&Name=${name}&IsPublic=${isPublic}`, {method: 'POST'}, false);
}

export async function RequestShufflePlaylist(guildId: number, playlistId: string): Promise<any> {
    return await apiRequest(`api/playlists/shuffle?GuildId=${guildId}&PlaylistId=${playlistId}`, {method: 'POST'}, false);
}

export async function RequestPlayPlaylist(guildId: number, playlistId: string, shouldPlay: boolean): Promise<any> {
    playlistId = encodeURIComponent(playlistId);
    return await apiRequest(`api/player/play-playlist?GuildId=${guildId}&PlaylistId=${playlistId}&ShouldPlay=${shouldPlay}`, {method: 'POST'}, false);
}

export async function RequestPlaylist(guildId: number, playlistId: string): Promise<any> {
    playlistId = encodeURIComponent(playlistId);
    return await apiRequest(`api/playlists/playlist?GuildId=${guildId}&PlaylistId=${playlistId}`, {method: 'GET'});
}

export async function RequestReorderPlaylist(guildId: number, playlistId: string, sourceIndex: number, destinationIndex: number): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    await apiRequest(`api/playlists/reorder?GuildId=${guildId}&playlistId=${playlistId}&sourceIndex=${sourceIndex}&destinationIndex=${destinationIndex}`, {method: 'POST'});
}

export async function RequestDeleteTrack(guildId: number, playlistId: string, trackId?: string, trackUrl?: string): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    if (trackId) {
        trackId = encodeURIComponent(trackId);
        await apiRequest(`api/playlists/delete-track?GuildId=${guildId}&PlaylistId=${playlistId}&trackId=${trackId}`, {method: 'POST'});
    } else if (trackUrl) {
        trackUrl = encodeURIComponent(trackUrl);
        await apiRequest(`api/playlists/delete-track?GuildId=${guildId}&PlaylistId=${playlistId}&trackUrl=${trackUrl}`, {method: 'POST'});
    }
}

export async function RequestAddTrack(guildId: number, playlistId: string, track: TrackDto): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    await apiRequest(`api/playlists/add-track?GuildId=${guildId}&PlaylistId=${playlistId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(track)
    });
}

export async function RequestAddPlaylist(guildId: number, playlistId: string, playlistUrl?: string, encodedTracks?: string[]): Promise<void> {
    playlistId = encodeURIComponent(playlistId);
    if (playlistUrl) {
        playlistUrl = encodeURIComponent(playlistUrl);
        await apiRequest(`api/playlists/add-playlist?GuildId=${guildId}&PlaylistId=${playlistId}&PlaylistUrl=${playlistUrl}`, {method: 'POST'});
    } else if (encodedTracks) {
        await apiRequest(`api/playlists/add-playlist?GuildId=${guildId}&PlaylistId=${playlistId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(encodedTracks)
        });
    }
}

export async function RequestAddTrackToPlaylistPreviews(guildId: number, trackUrl: string): Promise<any> {
    trackUrl = encodeURIComponent(trackUrl);
    return await apiRequest(`api/playlists/add-track-previews?GuildId=${guildId}&TrackUrl=${trackUrl}`, {method: 'GET'});
}


export async function RequestAddPlaylistToPlaylistPreviews(guildId: number, playlistUrl?: string, encodedTracks?: string[]): Promise<any> {
    if (playlistUrl) {
        playlistUrl = encodeURIComponent(playlistUrl);
        return await apiRequest(`api/playlists/add-playlist-previews?GuildId=${guildId}&PlaylistUrl=${playlistUrl}`, {method: 'GET'});
    } else if (encodedTracks) {
        encodedTracks = encodedTracks.map(encodeURIComponent);
        return await apiRequest(`api/playlists/add-playlist-previews?GuildId=${guildId}&encodedTracks=${encodedTracks.join(',')}`, {method: 'GET'});
    }
}