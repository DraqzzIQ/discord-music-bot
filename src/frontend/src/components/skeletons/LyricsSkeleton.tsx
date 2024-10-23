import {Skeleton} from "@/components/ui/skeleton";

function getRandomWidth() {
    const widths = ["w-1/2", "w-1/5", "w-1/6", "w-1/4", "w-1/3", "w-2/5", "w-3/5"];
    return widths[Math.floor(Math.random() * widths.length)];
}

export default function LyricsSkeleton() {
    return (
        <div className="items-center flex flex-col w-full">
            <Skeleton className={`h-4 ${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 ${getRandomWidth()} mt-1`}/>
        </div>
    )
}