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

export interface SearchTabProps {
    guildId: number
}

export default function SearchTab({guildId}: SearchTabProps) {
    const [searchResponse, setSearchResponse] = useState({} as SearchResponseDto | null)
    const [searchMode, setSearchMode] = useState(TrackSearchMode.Deezer.prefix)
    const [query, setQuery] = useState("")
    const [tmpQuery, setTmpQuery] = useState("")
    const [trackCount, setTrackCount] = useState(0)
    const [albumCount, setAlbumCount] = useState(0)
    const [playlistCount, setPlaylistCount] = useState(0)
    const [activeTab, setActiveTab] = useState("tracks")
    const onSearch = async (query: string, _searchMode?: string) => {
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
        if (response) {
            if (response.tracks.length > 0 && !response.albums.length && !response.playlists.length) {
                setActiveTab("tracks")
            } else if (!response.tracks.length && response.albums.length > 0 && !response.playlists.length) {
                setActiveTab("albums")
            } else if (!response.tracks.length && !response.albums.length && response.playlists.length > 0) {
                setActiveTab("playlists")
            } else {
                setActiveTab("tracks")
            }
        } else {
            setActiveTab("tracks")
        }

        console.log(response)
    }

    return (
        <div className="w-full mt-2 h-full">
            <div className="flex justify-center items-center w-full space-x-1">
                <SearchInput onSearch={onSearch} onChange={(query: string) => setTmpQuery(query)}
                             placeholder="What do you want play?"/>
                <SearchModeSelector onSelect={async (value) => {
                    setSearchMode(value);
                    await onSearch(tmpQuery, value)
                }}/>
                <InfoTooltip buttonLabel="Show help" popoverTitle="Choose Provider"
                             popoverContent="Choose the provider you want to search from or choose *Link* for links to playlists or songs."/>
            </div>
            <Tabs defaultValue="tracks" className="w-full px-2 pb-5 h-[calc(100%-88px)]" value={activeTab}
                  onValueChange={setActiveTab}>
                <div className="flex justify-center mt-3">
                    <TabsList>
                        <TabsTrigger value="tracks" disabled={trackCount == 0}>Tracks ({trackCount})</TabsTrigger>
                        <TabsTrigger value="albums" disabled={albumCount == 0}>Albums ({albumCount})</TabsTrigger>
                        <TabsTrigger value="playlists" disabled={playlistCount == 0}>Playlists
                            ({playlistCount})</TabsTrigger>
                    </TabsList>
                </div>
                <TabsContent value="tracks" className="w-full border-2 rounded-3xl h-full overflow-hidden pb-5">
                    <ScrollArea className="m-2 h-full overflow-hidden rounded-l-3xl">
                        <div className="space-y-1.5 overflow-auto">
                            {searchResponse?.tracks?.length ? (
                                searchResponse.tracks.map((track, index) => (
                                    <SearchTrack track={track} key={index}/>
                                ))
                            ) : query !== "" ? (
                                <div>No results found</div>
                            ) : <div/>}
                        </div>
                    </ScrollArea>
                </TabsContent>
                <TabsContent value="albums" className="w-full border-2 rounded-3xl h-full overflow-hidden pb-5">
                    <ScrollArea className="m-2 h-full overflow-hidden rounded-l-3xl">
                        <div className="space-y-1.5 overflow-auto">
                            {searchResponse?.albums?.length ? (
                                searchResponse.albums.map((album, index) => (
                                    <SearchPlaylist playlist={album} key={index}/>
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
                                    <SearchPlaylist playlist={playlist} key={index}/>
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