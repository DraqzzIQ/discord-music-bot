import PlaylistsTabToolBar from "@/components/dashboard/playlists/PlaylistsTabToolBar";
import React, {useEffect, useState} from "react";
import {RequestPinPlaylist, RequestPlaylist, RequestPlaylists, RequestUnpinPlaylist} from "@/api/rest/apiService";
import UserPlaylistPreview from "@/components/dashboard/playlists/UserPlaylistPreview";
import UserPlaylistPreviewDto from "@/dtos/UserPlaylistPreviewDto";
import Playlists from "@/components/dashboard/playlists/Playlists";
import PlaylistSkeleton from "@/components/skeletons/PlaylistSkeleton";
import UserPlaylist from "@/components/dashboard/playlists/UserPlaylist";
import UserPlaylistDto from "@/dtos/UserPlaylistDto";

interface PlaylistsTabProps {
    guildId: number;
    state: any;
    setState: (state: any) => void;
}

export default function PlaylistsTab({guildId, state, setState}: PlaylistsTabProps) {
    const [playlists, setPlaylists] = useState<UserPlaylistPreviewDto[]>(state?.playlists ?? []);
    const [currentPlaylist, setCurrentPlaylist] = useState<UserPlaylistDto | null>(state?.currentPlaylist);
    const [filteredPlaylists, setFilteredPlaylists] = useState<UserPlaylistPreviewDto[]>(state?.filteredPlaylists ?? []);
    const [loading, setLoading] = useState<boolean>(false);
    const [loadingPlaylist, setLoadingPlaylist] = useState<boolean>(false);
    const [ownFilter, setOwnFilter] = useState<boolean>(state?.ownFilter ?? false);
    const [query, setQuery] = useState<string>(state?.query ?? "");

    const fetchPlaylists = async () => {
        setLoading(true);

        const [response, response2] = await Promise.all([
            RequestPlaylists(guildId),
            currentPlaylist !== null && currentPlaylist !== undefined
                ? selectPlaylist(playlists.find(p => p.id == currentPlaylist.id)!)
                : Promise.resolve(null)
        ]);

        setPlaylists(response);
        updateFilteredPlaylists(query, response, ownFilter, response2);
        setLoading(false);
    }

    const onCheckedChange = (checked: boolean) => {
        setOwnFilter(checked);
        updateFilteredPlaylists(query, playlists, checked, currentPlaylist);
    }

    const onQueryChange = (query: string) => {
        setQuery(query);
        updateFilteredPlaylists(query, playlists, ownFilter, currentPlaylist);
    }

    const onPinChange = async (playlistId: string, isPinned: boolean) => {
        playlists.find((playlist) => playlist.id === playlistId)!.isPinned = isPinned;
        updateFilteredPlaylists(query, playlists, ownFilter, currentPlaylist);
        if (isPinned) {
            await RequestPinPlaylist(guildId, playlistId);
        } else {
            await RequestUnpinPlaylist(guildId, playlistId);
        }
    }

    const onRefresh = async () => {
        await fetchPlaylists();
    }

    const saveState = (query: string, onlyOwn: boolean, playlists: UserPlaylistPreviewDto[], filteredPlaylists: UserPlaylistPreviewDto[], userPlaylist: UserPlaylistDto | null) => {
        setState(
            {
                playlists: playlists,
                filteredPlaylists: filteredPlaylists,
                ownFilter: onlyOwn,
                query: query,
                currentPlaylist: userPlaylist,
            });
    }

    const updateFilteredPlaylists = (query: string, playlists: UserPlaylistPreviewDto[], onlyOwn: boolean, currentPlaylist: UserPlaylistDto | null) => {
        if (playlists === null) {
            setFilteredPlaylists(playlists);
            saveState(query, onlyOwn, playlists, playlists, currentPlaylist);
            return;
        }
        let filteredPlaylists = playlists.filter((playlist) => playlist.name.toLowerCase().includes(query.toLowerCase().trim()));
        if (onlyOwn) {
            filteredPlaylists = filteredPlaylists.filter((playlist) => playlist.isOwn);
        }
        // Sort by pinned status
        filteredPlaylists.sort((a, b) => {
            if (a.isPinned && !b.isPinned) {
                return -1;
            }
            if (!a.isPinned && b.isPinned) {
                return 1;
            }
            return 0;
        });
        setFilteredPlaylists(filteredPlaylists);
        saveState(query, onlyOwn, playlists, filteredPlaylists, currentPlaylist);
    }

    const selectPlaylist = async (playlist: UserPlaylistPreviewDto): Promise<UserPlaylistDto | null> => {
        setLoadingPlaylist(true);
        const response = await RequestPlaylist(guildId, playlist.id);
        setCurrentPlaylist(response);
        setLoadingPlaylist(false)
        saveState(query, ownFilter, playlists, filteredPlaylists, response);
        return response;
    }

    const onGoBack = () => {
        setCurrentPlaylist(null);
        saveState(query, ownFilter, playlists, filteredPlaylists, null);
    }

    useEffect(() => {
        if (playlists.length === 0)
            fetchPlaylists();
    }, [guildId]);

    return (
        <div className="mt-2 space-y-3 h-[calc(100%-60px)]">
            {currentPlaylist || loadingPlaylist ? (
                <UserPlaylist playlist={currentPlaylist} goBack={onGoBack}
                              guildId={guildId} onRefresh={onRefresh}/>
            ) : (
                <>
                    <PlaylistsTabToolBar
                        onCheckedChange={onCheckedChange}
                        onQueryChange={onQueryChange}
                        onRefresh={onRefresh}
                        guildId={guildId}
                        checked={ownFilter}
                        query={query}
                    />
                    {loading ? (
                        <Playlists>
                            {Array.from({length: 10}, (_, index) => (
                                <PlaylistSkeleton key={index}/>
                            ))}
                        </Playlists>
                    ) : filteredPlaylists !== null && filteredPlaylists.length > 0 ? (
                        <Playlists>
                            {filteredPlaylists.map((playlist: UserPlaylistPreviewDto) => (
                                <div key={playlist.id} className="w-[160px]">
                                    <UserPlaylistPreview
                                        playlist={playlist}
                                        onPinChange={onPinChange}
                                        guildId={guildId}
                                        refresh={onRefresh}
                                        selectPlaylist={async (playlist) => {
                                            await selectPlaylist(playlist)
                                        }}
                                    />
                                </div>
                            ))}
                        </Playlists>
                    ) : (
                        <div className="text-center w-full">No Playlists</div>
                    )}
                </>
            )}
        </div>
    );
}