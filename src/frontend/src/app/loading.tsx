import GuildSkeleton from "@/components/skeletons/GuildSkeleton";
import {ScrollArea} from "@/components/ui/scroll-area";
import React from "react";

function Loading() {
    return (
        <div>
            <div className="text-center my-4">
                <h1 className="text-3xl font-semibold">Select Server</h1>
            </div>
            <ScrollArea className="p-4 overflow-y-auto flex-grow">
                <div className="grid-responsive justify-center">
                    {Array.from({length: 3}, (_, index) => (
                        <GuildSkeleton key={index}/>
                    ))}
                </div>
            </ScrollArea>
        </div>

    );
}

export default Loading;