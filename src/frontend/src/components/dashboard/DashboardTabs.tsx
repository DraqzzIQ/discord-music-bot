import { useState } from "react";
import { VerticalTabs, VerticalTabsContent, VerticalTabsList, VerticalTabsTrigger } from "@/components/ui/tabs-vertical";
import PlaylistTab from "@/components/dashboard/PlaylistTab";
import SearchTab from "@/components/dashboard/SearchTab";
import LyricsTab from "@/components/dashboard/LyricsTab";
import { TrackDto } from "@/dtos/TrackDto";

export interface DashboardTabsProps {
    guildId: number;
    track: TrackDto | null;
}

export default function DashboardTabs({ guildId, track }: DashboardTabsProps) {
    const [searchState, setSearchState] = useState(null);
    const [lyricsState, setLyricsState] = useState(null);
    const [playlistState, setPlaylistState] = useState(null);

    return (
        <div className="p-3 w-2/3">
            <VerticalTabs defaultValue="search" className="flex-grow h-full items-center w-full">
                <VerticalTabsList className="">
                    <VerticalTabsTrigger value="search">Search</VerticalTabsTrigger>
                    <VerticalTabsTrigger value="lyrics">Lyrics</VerticalTabsTrigger>
                    <VerticalTabsTrigger value="playlists" disabled={true}>Playlists</VerticalTabsTrigger>
                </VerticalTabsList>
                <VerticalTabsContent value="search" className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <SearchTab guildId={guildId} state={searchState} setState={setSearchState} />
                </VerticalTabsContent>
                <VerticalTabsContent value="lyrics" className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <LyricsTab guildId={guildId} track={track} state={lyricsState} setState={setLyricsState} />
                </VerticalTabsContent>
                <VerticalTabsContent value="playlists" className="flex-grow h-full border-2 rounded-3xl max-w-[calc(100%-100px)]">
                    <PlaylistTab state={playlistState} setState={setPlaylistState} />
                </VerticalTabsContent>
            </VerticalTabs>
        </div>
    );
}