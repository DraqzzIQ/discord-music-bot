import {Skeleton} from "@/components/ui/skeleton"

const QueuedSongSkeleton = () => {
    return (
        <div className="flex flex-row space-x-1.5">
            <Skeleton className="h-[65px] w-[65px] rounded-xl"/>
            <div className="space-y-2 content-center">
                <Skeleton className="h-4 w-[180px]"/>
                <Skeleton className="h-3.5 w-[120px]"/>
            </div>
        </div>
    )
}

export default QueuedSongSkeleton;