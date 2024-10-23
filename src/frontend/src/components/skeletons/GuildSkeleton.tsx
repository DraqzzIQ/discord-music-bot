import {Skeleton} from "@/components/ui/skeleton"

const GuildSkeleton = () => {
    return (
        <div className="space-y-2 items-center flex flex-col">
            <Skeleton className="h-[150px] w-[150px] rounded-full"/>
            <Skeleton className="h-6 w-[150px]"/>
        </div>
    )
}

export default GuildSkeleton;