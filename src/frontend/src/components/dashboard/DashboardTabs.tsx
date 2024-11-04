import {useState} from "react";
import {VerticalTabs, VerticalTabsContent, VerticalTabsList, VerticalTabsTrigger} from "@/components/ui/tabs-vertical";
import PlaylistTab from "@/components/dashboard/PlaylistsTab";
import SearchTab from "@/components/dashboard/SearchTab";
import LyricsTab from "@/components/dashboard/LyricsTab";
import {TrackDto} from "@/dtos/TrackDto";
import {LibraryIcon, SearchIcon, TextIcon} from "lucide-react";

export interface DashboardTabsProps {
    guildId: number;
    track: TrackDto | null;
}

export default function DashboardTabs({guildId, track}: DashboardTabsProps) {
    const [searchState, setSearchState] = useState(null);
    const [lyricsState, setLyricsState] = useState(null);
    const [playlistState, setPlaylistState] = useState(null);

    return (
        <div className="p-3 flex-grow">
            <VerticalTabs defaultValue="search" className="flex-grow h-full items-center w-full">
                <VerticalTabsList>
                    <VerticalTabsTrigger value="search" className="w-full">
                        <SearchIcon className="w-5 h-5 mr-1"/>
                        <div className="mr-auto">
                            Search
                        </div>
                    </VerticalTabsTrigger>
                    <VerticalTabsTrigger value="lyrics" className="w-full">
                        <TextIcon className="w-5 h-5 mr-1"/>
                        <div className="mr-auto">
                            Lyrics
                        </div>
                    </VerticalTabsTrigger>
                    <VerticalTabsTrigger value="playlists" className="w-full">
                        <LibraryIcon className="w-5 h-5 mr-1"/>
                        <div className="mr-auto">
                            Playlists
                        </div>
                    </VerticalTabsTrigger>
                </VerticalTabsList>
                <VerticalTabsContent value="search"
                                     className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <SearchTab guildId={guildId} state={searchState} setState={setSearchState}/>
                </VerticalTabsContent>
                <VerticalTabsContent value="lyrics"
                                     className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <LyricsTab guildId={guildId} track={track} state={lyricsState} setState={setLyricsState}/>
                </VerticalTabsContent>
                <VerticalTabsContent value="playlists"
                                     className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <PlaylistTab guildId={guildId} state={playlistState} setState={setPlaylistState}/>
                </VerticalTabsContent>
            </VerticalTabs>
        </div>
    );
}