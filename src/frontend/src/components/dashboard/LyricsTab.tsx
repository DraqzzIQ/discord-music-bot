import {TrackDto} from "@/dtos/TrackDto";
import React from "react";
import {RequestLyrics} from "@/api/rest/apiService";
import {ScrollArea} from "@/components/ui/scroll-area";
import LyricsSkeleton from "@/components/skeletons/LyricsSkeleton";

export interface LyricsTabProps {
    guildId: number;
    track: TrackDto | null;
}

export default function LyricsTab({guildId, track}: LyricsTabProps) {
    const [lyrics, setLyrics] = React.useState<string | null>(null);
    const [loading, setLoading] = React.useState(true);

    React.useEffect(() => {
        async function getLyrics() {
            const response = await RequestLyrics(guildId);
            if (response) {
                setLyrics(response.lyrics.text);
            }
            setLoading(false);
        }

        getLyrics();
    }, [guildId]);

    return (
        <div className="text-center mt-4 h-full w-full">
            <div className="font-bold text-3xl">Lyrics</div>
            <div>{track ? <>Lyrics for <strong>{track?.title}</strong></> : "No Song playing"}</div>
            <div className="h-[calc(100%-110px)] mx-1">
                <ScrollArea className="mt-4 h-full w-full">
                    {loading &&
                        <LyricsSkeleton/>
                    }
                    {!loading &&
                        <div className="whitespace-pre-wrap">{lyrics ?? "No lyrics available for this song"}</div>
                    }
                </ScrollArea>
            </div>
        </div>
    )
}