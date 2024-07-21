"use client";

import React from "react";
import {
    ChevronLeft,
    ChevronRight,
    LucideVolume,
    TriangleIcon
} from "lucide-react";
import {Slider} from "@/components/ui/slider";
import DefaultButton from "@/components/DefaultButton";
import {Queue} from "@phosphor-icons/react";
import QueuedSongSkeleton from "@/components/sceletons/QueuedSongSkeleton";

interface PlayerControlProps {
    toggleQueue: () => void;
}

const PlayerControl: React.FC<PlayerControlProps> = ({toggleQueue}) => {
    return (
        <div className="flex flex-row justify-center items-center ml-2 mb-0.5 text-center border-t-2 mr-2">
            <QueuedSongSkeleton/>
            <div className="felx flex-row w-full">
                <div className="flex items-center justify-center">
                    <DefaultButton tooltipText="Previous">
                        <ChevronLeft className="w-10 h-10"/>
                    </DefaultButton>
                    <DefaultButton tooltipText="Play/Pause">
                        <TriangleIcon className="rotate-90"/>
                    </DefaultButton>
                    <DefaultButton tooltipText="Next">
                        <ChevronRight className="w-10 h-10"/>
                    </DefaultButton>
                </div>
                <div className="flex justify-center">
                    <div className="flex items-center justify-center w-2/3 pb-2 select-none">
                        01:10
                        <div className="w-10/12 min-w-56 px-4">
                            <Slider defaultValue={[33]} max={100} step={1}/>
                        </div>
                        03:30
                    </div>
                </div>
            </div>
            <div className="flex w-96 items-center justify-end">
                <DefaultButton tooltipText={"Open/Close Queue"} onClick={toggleQueue} className="mr-3">
                    <Queue size="22"/>
                </DefaultButton>
                <DefaultButton tooltipText="Mute/Unmute" className="m-0" disabled={true}>
                    <LucideVolume
                        className="bg-transparent text-foreground pl-2 w-8 h-8"/>
                </DefaultButton>
                <Slider defaultValue={[33]} max={100} step={1} className="min-w-20 max-w-32" disabled={true}/>
                <div className="ml-3 opacity-50 select-none">
                    100%
                </div>
            </div>
        </div>
    );
}

export default PlayerControl;