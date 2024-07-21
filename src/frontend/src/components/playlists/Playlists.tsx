import React from "react";
import PlaylistSkeleton from "@/components/sceletons/PlaylistSkeleton";
import {ScrollArea} from "@/components/ui/scroll-area";

const Playlists: React.FC = () => {
    return (
        <ScrollArea className="p-4 overflow-y-auto flex-grow ">
            <div className="grid-responsive">
                {Array.from({ length: 10 }, (_, index) => (
                    <PlaylistSkeleton key={index} />
                ))}
            </div>
        </ScrollArea>
    );
};

export default Playlists;