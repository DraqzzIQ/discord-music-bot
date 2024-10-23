import React, {useState, useEffect} from "react";
import {ScrollArea} from "@/components/ui/scroll-area";
import LyricsSkeleton from "@/components/skeletons/LyricsSkeleton";
import {RequestLyrics} from "@/api/rest/apiService";

export interface LyricsTabProps {
    guildId: number;
    track: TrackDto | null;
    state: any;
    setState: (state: any) => void;
}

export default function LyricsTab({guildId, track, state, setState}: LyricsTabProps) {
    const [lyrics, setLyrics] = useState<string | null>(state?.lyrics || null);
    const [loading, setLoading] = useState(state?.loading || false);

    async function getLyrics() {
        if (track !== state?.track && track) {
            const response = await RequestLyrics(guildId);
            if (response) {
                setLyrics(response?.lyrics?.text);
            }
            setState({lyrics: response?.lyrics?.text, loading: false, track: track});
        }
        setLoading(false);
    }

    useEffect(() => {
        if (state?.track !== track) {
            setLoading(true);
        }
        getLyrics();
    }, [track?.title, track?.author, track?.url]);

    return (
        <div className="text-center mt-4 h-full w-full">
            <div className="font-bold text-3xl">Lyrics</div>
            <div>{track ? <>Lyrics for <strong>{track?.title}</strong></> : "No Song playing"}</div>
            <div className="h-[calc(100%-110px)] mx-1">
                <ScrollArea className="h-full w-full">
                    {loading &&
                        <LyricsSkeleton/>
                    }
                    {!loading &&
                        <div className="whitespace-pre-wrap mt-4">{lyrics ?? "No lyrics available for this song"}</div>
                    }
                </ScrollArea>
            </div>
        </div>
    );
}