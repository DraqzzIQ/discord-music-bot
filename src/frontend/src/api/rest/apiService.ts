import {GuildDto} from "@/dtos/GuildDto";
import {SearchResponseDto} from "@/dtos/SearchResponseDto";
import {PlayRequestDto} from "@/dtos/PlayRequestDto";

// make API requests with cookie authentication
async function apiRequest(url: string, options: RequestInit = {}, parse: boolean = true): Promise<any> {
    const response = await fetch(process.env.NEXT_PUBLIC_REST_API_URL + url, {
        credentials: 'include',
        ...options,
    });

    // log if debug
    if (!response.ok && process.env.NODE_ENV === 'development') {
        console.log(`API request failed with status ${response.status} and body ${await response.text()} for URL ${url}`);
        return null;
    }
    
    const text = await response.text();
    
    if (!parse) {
        return text;
    }
    
    return text ? JSON.parse(text) : null;
}

export async function RequestGuilds():Promise<GuildDto[]> {
    const data = await apiRequest('api/discord/guilds');
    
    return data.guilds.map((guild: any): GuildDto => ({
        id: guild.id,
        name: guild.name,
        iconUrl: guild.iconUrl,
    }));
}

export async function RequestRewind(guildId: number):Promise<void> {
    await apiRequest(`api/bot/rewind?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestSkip(guildId: number, index: number = 0):Promise<void> {
    await apiRequest(`api/bot/skip?GuildId=${guildId}&Index=${index}`, { method: 'POST' });
}

export async function RequestPause(guildId: number):Promise<void> {
    await apiRequest(`api/bot/pause?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestResume(guildId: number):Promise<void> {
    await apiRequest(`api/bot/resume?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestSeek(guildId: number, position: number):Promise<void> {
    await apiRequest(`api/bot/position?GuildId=${guildId}&positionInSeconds=${position}`, {method: 'POST' });
}

export async function RequestShuffle(guildId: number):Promise<void> {
    await apiRequest(`api/bot/shuffle?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestClear(guildId: number):Promise<void> {
    await apiRequest(`api/bot/clear?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestDeduplicate(guildId: number):Promise<void> {
    await apiRequest(`api/bot/deduplicate?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestRemove(guildId: number, index: number):Promise<void> {
    await apiRequest(`api/bot/remove?GuildId=${guildId}&index=${index}`, { method: 'POST' });
}

export async function RequestReorder(guildId: number, sourceIndex: number, destinationIndex: number):Promise<void> {
    await apiRequest(`api/bot/reorder?GuildId=${guildId}&sourceIndex=${sourceIndex}&destinationIndex=${destinationIndex}`, {
        method: 'POST',
    });
}

export async function RequestSearch(guildId: number, query: string, searchMode: string):Promise<SearchResponseDto | null> {
    const urlEncodedQuery = encodeURIComponent(query);
    return await apiRequest(`api/bot/search?GuildId=${guildId}&query=${urlEncodedQuery}&searchMode=${searchMode}`, { method: 'GET' });
}

export async function RequestPlay(guildId: number, playRequest: PlayRequestDto):Promise<boolean> {
    return await apiRequest(`api/bot/play?GuildId=${guildId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(playRequest),
    }, false) == "";
}

export async function RequestLeave(guildId: number):Promise<void> {
    await apiRequest(`api/bot/leave?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestStop(guildId: number):Promise<void> {
    await apiRequest(`api/bot/stop?GuildId=${guildId}`, { method: 'POST' });
}

export async function RequestLyrics(guildId: number):Promise<any> {
    return await apiRequest(`api/bot/lyrics?GuildId=${guildId}`, { method: 'GET' });
}