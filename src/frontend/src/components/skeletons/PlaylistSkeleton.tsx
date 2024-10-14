import {Skeleton} from "@/components/ui/skeleton"

const PlaylistSkeleton = () => {
    return (
        <div className="flex flex-col space-y-3">
            <Skeleton className="h-[200px] w-[200px] rounded-xl"/>
            <div className="space-y-2">
                <Skeleton className="h-4 w-[200px]"/>
                <Skeleton className="h-3.5 w-[150px]"/>
            </div>
        </div>
    )
}

export default PlaylistSkeleton;