import React from "react";
import {ScrollArea} from "@/components/ui/scroll-area";

interface PlaylistsProps {
    children?: React.ReactNode;
}

const Playlists: React.FC<PlaylistsProps> = ({children}) => {
    return (
        <div className="h-full w-full pr-4 ml-2 pb-2">
            <ScrollArea className="h-full w-full rounded-l-2xl">
                <div className="grid-responsive mt-1">
                    {children}
                </div>
            </ScrollArea>
        </div>
    );
};

export default Playlists;