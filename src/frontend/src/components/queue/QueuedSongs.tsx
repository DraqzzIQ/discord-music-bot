import React from "react";
import {ScrollArea} from "@/components/ui/scroll-area";
import QueuedSongSkeleton from "@/components/sceletons/QueuedSongSkeleton";

const QueuedSongs: React.FC = () => {
    return (
        <ScrollArea className="w-72 border-l-2 p-4 overflow-y-auto mt-2">
            <div className="space-y-2">
                {Array.from({length: 30}, (_, index) => (
                    <QueuedSongSkeleton key={index}/>
                ))}
            </div>
        </ScrollArea>
    );
};

export default QueuedSongs;