import React from "react";
import PlaylistSkeleton from "@/components/skeletons/PlaylistSkeleton";
import {ScrollArea} from "@/components/ui/scroll-area";

interface PlaylistsProps {
    children?: React.ReactNode;
}

const Playlists: React.FC<PlaylistsProps> = ({ children }) => {
    return (
        <ScrollArea className="p-4 overflow-y-auto flex-grow ">
            <div className="grid-responsive">
                {children}
            </div>
        </ScrollArea>
    );
};

export default Playlists;