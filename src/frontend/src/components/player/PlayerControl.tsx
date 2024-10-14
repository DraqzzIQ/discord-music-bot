"use client";

import React, {useEffect, useState} from "react";
import {ChevronLeft, ChevronRight, PauseIcon, TriangleIcon} from "lucide-react";
import {Slider} from "@/components/ui/slider";
import DefaultButton from "@/components/DefaultButton";
import {Queue} from "@phosphor-icons/react";
import {TrackDto} from "@/dtos/TrackDto";
import {formatDuration} from "@/lib/utils";
import {RequestPause, RequestResume, RequestRewind, RequestSeek, RequestSkip} from "@/api/rest/apiService";
import {PlayerState} from "@/datatypes/PlayerState";

interface PlayerControlProps {
    toggleQueue: () => void;
    track: TrackDto | null;
    positionInSeconds: number;
    guildId: number;
    state: PlayerState;
}

const PlayerControl: React.FC<PlayerControlProps> = ({
                                                         toggleQueue,
                                                         track,
                                                         positionInSeconds,
                                                         guildId,
                                                         state
                                                     }) => {
    const [sliderValue, setSliderValue] = useState(positionInSeconds);
    const [currentTrack, setCurrentTrack] = useState(track);
    const [playerState, setPlayerState] = useState(state);

    useEffect(() => {
        setSliderValue(positionInSeconds);
        setCurrentTrack(track);
        setPlayerState(state);

        if (track && state === PlayerState.Playing) {
            const interval = setInterval(() => {
                setSliderValue((p) => p + 1);
            }, 1000);
            return () => clearInterval(interval);
        }
    }, [positionInSeconds, track, state]);


    const handleSliderChange = async (value: number[]) => {
        setSliderValue(value[0]);
    };
    const handleSliderCommit = async (value: number[]) => {
        setSliderValue(value[0]);
        await RequestSeek(guildId, value[0]);
    };

    return (
        <div className="flex flex-row justify-center items-center ml-2 mb-0.5 text-center border-t-2 mr-2 border-white mt-1 pt-2">
            <div className="flex flex-row space-x-3 w-1/3">
                <img
                    className={`h-[60px] w-[60px] rounded-xl object-cover ${currentTrack?.thumbnailUrl ? '' : 'dark:invert'}`}
                    src={currentTrack?.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                    alt='thumbnail'
                />
                <div className="space-y-2 w-full text-left">
                    <div className="w-full truncate">
                        <a href={currentTrack?.url} target="_blank" rel="noreferrer"
                           className="hover:underline font-semibold text-xl">
                            {currentTrack?.title ?? "No Track Playing"}
                        </a>
                    </div>
                    <div className="text-gray-400 truncate">{currentTrack?.author ?? "No Track Playing"}</div>
                </div>
            </div>
            <div className="felx flex-row w-1/3">
                <div className="flex items-center justify-center">
                    <DefaultButton tooltipText="Previous" onClick={() => RequestRewind(guildId)}>
                        <ChevronLeft className="w-10 h-10"/>
                    </DefaultButton>
                    <DefaultButton tooltipText="Play/Pause" onClick={
                        () => {
                            playerState === PlayerState.Playing ? RequestPause(guildId) : RequestResume(guildId);
                        }
                    }>
                        {playerState === PlayerState.Playing ? <PauseIcon className="h-8 w-8"/> : <TriangleIcon className="h-8 w-8 rotate-90"/>}
                    </DefaultButton>
                    <DefaultButton tooltipText="Next" onClick={() => RequestSkip(guildId)}>
                        <ChevronRight className="w-10 h-10"/>
                    </DefaultButton>
                </div>
                <div className="flex justify-center">
                    <div className="flex items-center justify-center w-11/12 pb-2 select-none">
                        {formatDuration(sliderValue ?? 0)}
                        <div className="w-full min-w-56 px-4">
                            <Slider
                                value={[sliderValue ?? 0]}
                                max={currentTrack?.durationInSeconds ?? 100}
                                step={1}
                                onValueChange={handleSliderChange}
                                onValueCommit={handleSliderCommit}
                            />
                        </div>
                        {formatDuration(currentTrack?.durationInSeconds ?? 0)}
                    </div>
                </div>
            </div>
            <div className="flex w-1/3 items-center justify-end">
                <DefaultButton tooltipText={"Open/Close Queue"} onClick={toggleQueue} className="mr-3">
                    <Queue size="22"/>
                </DefaultButton>
                {/* too much trolling */}
                {/*<DefaultButton tooltipText="Mute/Unmute" className="m-0" disabled={true}>*/}
                {/*    <LucideVolume*/}
                {/*        className="bg-transparent text-foreground pl-2 w-8 h-8"/>*/}
                {/*</DefaultButton>*/}
                {/*<Slider defaultValue={[33]} max={100} step={1} className="min-w-20 max-w-32" disabled={true}/>*/}
                {/*<div className="ml-3 opacity-50 select-none">*/}
                {/*    100%*/}
                {/*</div>*/}
            </div>
        </div>
    );
}

export default PlayerControl;