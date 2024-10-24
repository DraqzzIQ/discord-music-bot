import SearchInput from "@/components/ui/search-input";
import {useState} from "react";
import {SearchModeSelector} from "@/components/dashboard/search/SearchModeSelector";
import {TrackSearchMode} from "@/datatypes/TrackSearchMode";
import {RequestSearch} from "@/api/rest/apiService";
import {Tabs, TabsContent, TabsTrigger, TabsList} from "@/components/ui/tabs";
import {SearchResponseDto} from "@/dtos/SearchResponseDto";
import SearchTrack from "@/components/dashboard/search/SearchTrack";
import {ScrollArea} from "@/components/ui/scroll-area";
import InfoTooltip from "@/components/ui/info-tooltip";
import SearchPlaylist from "@/components/dashboard/search/SearchPlaylist";
import QueuedSongSkeleton from "@/components/skeletons/QueuedSongSkeleton";
import {NotConnectedAlert} from "@/components/dashboard/search/NotConnectedAlert";

export interface SearchTabProps {
    guildId: number;
    state: any;
    setState: (state: any) => void;
}

export default function SearchTab({guildId, state, setState}: SearchTabProps) {
    const [searchResponse, setSearchResponse] = useState(state?.searchResponse as SearchResponseDto | null)
    const [searchMode, setSearchMode] = useState(state?.searchMode ?? TrackSearchMode.Deezer.prefix)
    const [query, setQuery] = useState(state?.query ?? "")
    const [tmpQuery, setTmpQuery] = useState(state?.tmpQuery ?? "")
    const [trackCount, setTrackCount] = useState(state?.trackCount ?? 0)
    const [albumCount, setAlbumCount] = useState(state?.albumCount ?? 0)
    const [playlistCount, setPlaylistCount] = useState(state?.playlistCount ?? 0)
    const [activeTab, setActiveTab] = useState(state?.activeTab ?? "tracks")
    const [loading, setLoading] = useState(state?.loading ?? false)
    const [errorPlaying, setErrorPlaying] = useState(false);
    const [forceRerender, setForceRerender] = useState(0);

    const onSearch = async (query: string, _searchMode?: string) => {
        setActiveTab("tracks")
        setLoading(true)
        const response = await RequestSearch(guildId, query, _searchMode ?? searchMode)
        setSearchResponse(response)
        setQuery(query)

        if (response) {
            setTrackCount(response.tracks.length)
            setAlbumCount(response.albums.length)
            setPlaylistCount(response.playlists.length)
        } else {
            setTrackCount(0)
            setAlbumCount(0)
            setPlaylistCount(0)
        }

        // if only 1 category has results, switch to that category
        let activeTab = "tracks"
        if (response) {
            if (!response.tracks.length && response.albums.length > 0 && !response.playlists.length) {
                activeTab = "albums"
            } else if (!response.tracks.length && !response.albums.length && response.playlists.length > 0) {
                activeTab = "playlists"
            }
        }
        setActiveTab(activeTab)
        setLoading(false)

        setState({
            searchResponse: response,
            searchMode: _searchMode ?? searchMode,
            query: query,
            tmpQuery: query,
            trackCount: response?.tracks?.length ?? 0,
            albumCount: response?.albums?.length ?? 0,
            playlistCount: response?.playlists?.length ?? 0,
            activeTab: activeTab,
            loading: false,
        })
    }

    return (
        <div className="w-full mt-2 h-full">
            {errorPlaying && <NotConnectedAlert key={forceRerender}/>}
            <div className="flex justify-center items-center w-full space-x-1">
                <SearchInput passedQuery={query} onSearch={onSearch} onChange={(query: string) => {
                    setTmpQuery(query)
                    setState({
                        searchResponse: searchResponse,
                        searchMode: searchMode,
                        query: query,
                        tmpQuery: query,
                        trackCount: trackCount,
                        albumCount: albumCount,
                        playlistCount: playlistCount,
                        activeTab: activeTab,
                        loading: loading,
                    })
                }}
                             placeholder="What do you want play?"/>
                <SearchModeSelector passedValue={searchMode} onSelect={async (value) => {
                    setSearchMode(value);
                    await onSearch(tmpQuery, value)
                }}/>
                <InfoTooltip buttonLabel="Show help" popoverTitle="Choose Provider"
                             popoverContent="Choose the provider you want to search from or choose *Link* for links to playlists or songs."/>
            </div>
            <Tabs defaultValue="tracks" className="w-full px-2 pb-5 h-[calc(100%-88px)]" value={activeTab}
                  onValueChange={async (value) => {
                      setActiveTab(value)
                      setState({
                          searchResponse: searchResponse,
                          searchMode: searchMode,
                          query: query,
                          tmpQuery: tmpQuery,
                          trackCount: trackCount,
                          albumCount: albumCount,
                          playlistCount: playlistCount,
                          activeTab: value,
                          loading: loading,
                      })
                  }}>
                <div className="flex justify-center mt-3">
                    <TabsList>
                        <TabsTrigger value="tracks" disabled={trackCount == 0}>Songs ({trackCount})</TabsTrigger>
                        <TabsTrigger value="albums" disabled={albumCount == 0}>Albums ({albumCount})</TabsTrigger>
                        <TabsTrigger value="playlists" disabled={playlistCount == 0}>Playlists
                            ({playlistCount})</TabsTrigger>
                    </TabsList>
                </div>
                <TabsContent value="tracks" className="w-full border-2 rounded-3xl h-full overflow-hidden pb-5">
                    <ScrollArea className="m-2 h-full overflow-hidden rounded-l-3xl">
                        <div className="space-y-1.5 overflow-auto">
                            {loading && Array.from({length: 20}).map((_, index) => (
                                <QueuedSongSkeleton key={index}/>
                            ))}
                            {!loading && searchResponse?.tracks?.length ? (
                                searchResponse.tracks.map((track, index) => (
                                    <SearchTrack track={track} key={index} guildId={guildId}
                                                 setOnErrorPlaying={() => {
                                                     setErrorPlaying(true);
                                                     setForceRerender(prev => prev + 1);
                                                 }}/>
                                ))
                            ) : !loading ? (
                                <div className="text-center">No results found</div>
                            ) : <div/>}
                        </div>
                    </ScrollArea>
                </TabsContent>
                <TabsContent value="albums" className="w-full border-2 rounded-3xl h-full overflow-hidden pb-5">
                    <ScrollArea className="m-2 h-full overflow-hidden rounded-l-3xl">
                        <div className="space-y-1.5 overflow-auto">
                            {searchResponse?.albums?.length ? (
                                searchResponse.albums.map((album, index) => (
                                    <SearchPlaylist playlist={album} key={index} guildId={guildId}
                                                    setOnErrorPlaying={() => {
                                                        setErrorPlaying(true);
                                                        setForceRerender(prev => prev + 1);
                                                    }}/>
                                ))
                            ) : query !== "" ? (
                                <div>No results found</div>
                            ) : <div/>}
                        </div>
                    </ScrollArea>
                </TabsContent>
                <TabsContent value="playlists" className="w-full border-2 rounded-3xl h-full overflow-hidden pb-5">
                    <ScrollArea className="m-2 h-full overflow-hidden rounded-l-3xl">
                        <div className="space-y-1.5 overflow-auto">
                            {searchResponse?.playlists?.length ? (
                                searchResponse.playlists.map((playlist, index) => (
                                    <SearchPlaylist playlist={playlist} key={index} guildId={guildId}
                                                    setOnErrorPlaying={() => {
                                                        setErrorPlaying(true);
                                                        setForceRerender(prev => prev + 1);
                                                    }}/>
                                ))
                            ) : query !== "" ? (
                                <div>No results found</div>
                            ) : <div/>}
                        </div>
                    </ScrollArea>
                </TabsContent>
            </Tabs>
        </div>
    )
}