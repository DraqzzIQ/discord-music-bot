import {Skeleton} from "@/components/ui/skeleton"
import QueuedSongSkeleton from "@/components/skeletons/QueuedSongSkeleton";

export default function UserPlaylistSkeleton() {
    return (
        <div className="w-full mt-3 ml-3">
            <Skeleton className="h-6 w-2/3"/>
            <Skeleton className="h-4 w-1/2 mt-3"/>
            <Skeleton className="h-3 w-1/3 mt-2.5"/>
            <Skeleton className="h-3 w-1/4 mt-2.5"/>

            <div className="space-y-3.5 mt-9">
                <QueuedSongSkeleton/>
                <QueuedSongSkeleton/>
                <QueuedSongSkeleton/>
                <QueuedSongSkeleton/>
                <QueuedSongSkeleton/>
                <QueuedSongSkeleton/>
            </div>
        </div>
    );
}