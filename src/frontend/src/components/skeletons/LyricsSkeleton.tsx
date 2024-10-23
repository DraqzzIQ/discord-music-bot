import {Skeleton} from "@/components/ui/skeleton";
import React from "react";

function getRandomWidth(previousWidth: string | null) {
    const widths = ["w-1/5", "w-1/6", "w-1/4", "w-1/3", "w-2/5"];
    const excludedWidths = previousWidth ? widths.filter(width => width !== previousWidth) : widths;
    
    return excludedWidths[Math.floor(Math.random() * excludedWidths.length)];
}

export default function LyricsSkeleton() {
    let previousWidth: null | string = null;

    const skeletons = Array.from({length: 32}).map((_, index) => {
        const width = getRandomWidth(previousWidth);
        previousWidth = width;
        const marginTop = index % 6 === 0 ? 'mt-10' : 'mt-1';
        return <Skeleton key={index} className={`h-4 ${width} ${marginTop}`}/>;
    });

    return (
        <div className="items-center flex flex-col w-full">
            {skeletons}
        </div>
    );
}