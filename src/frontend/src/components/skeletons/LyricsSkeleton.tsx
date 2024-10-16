import {Skeleton} from "@/components/ui/skeleton";

function getRandomWidth() {
    const widths = ["1/2", "2/7", "2/6", "1/5", "1/6", "1/4", "1/3", "2/5", "3/5", "2/6"];
    return widths[Math.floor(Math.random() * widths.length)];
}

export default function LyricsSkeleton() {
    return (
        <div className="items-center justify-center flex flex-col">
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-4`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-10`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
            <Skeleton className={`h-4 w-${getRandomWidth()} mt-1`}/>
        </div>
    )
}