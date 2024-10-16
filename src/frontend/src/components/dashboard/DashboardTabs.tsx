import {VerticalTabs, VerticalTabsContent, VerticalTabsList, VerticalTabsTrigger} from "@/components/ui/tabs-vertical";
import PlaylistTab from "@/components/dashboard/PlaylistTab";
import SearchTab from "@/components/dashboard/SearchTab";
import LyricsTab from "@/components/dashboard/LyricsTab";
import {TrackDto} from "@/dtos/TrackDto";

export interface DashboardTabsProps {
    guildId: number;
    track: TrackDto | null;
}

export default function DashboardTabs({guildId, track}: DashboardTabsProps) {
    return (
        <VerticalTabs defaultValue="search" className="flex-grow h-full items-center p-3">
            <VerticalTabsList className="">
                <VerticalTabsTrigger value="search">Search</VerticalTabsTrigger>
                <VerticalTabsTrigger value="lyrics">Lyrics</VerticalTabsTrigger>
                <VerticalTabsTrigger value="playlists" disabled={true}>Playlists</VerticalTabsTrigger>
            </VerticalTabsList>
            <VerticalTabsContent value="search" className="w-full h-full border-2 rounded-3xl">
                <SearchTab guildId={guildId}/>
            </VerticalTabsContent>
            <VerticalTabsContent value="lyrics" className="w-full h-full border-2 rounded-3xl">
                <LyricsTab guildId={guildId} track={track}/>
            </VerticalTabsContent>
            <VerticalTabsContent value="playlists" className="w-full h-full border-2 rounded-3xl">
                <PlaylistTab/>
            </VerticalTabsContent>
        </VerticalTabs>
    )
}