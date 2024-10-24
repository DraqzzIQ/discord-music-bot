"use client";

import React, {useEffect, useState} from "react";
import {ChevronLeft, ChevronRight, Loader2, PauseIcon, TriangleIcon} from "lucide-react";
import {Slider} from "@/components/ui/slider";
import DefaultButton from "@/components/DefaultButton";
import {Door, Queue, Stop} from "@phosphor-icons/react";
import {TrackDto} from "@/dtos/TrackDto";
import {formatDuration} from "@/lib/utils";
import {
    RequestLeave,
    RequestPause,
    RequestResume,
    RequestRewind,
    RequestSeek,
    RequestSkip,
    RequestStop
} from "@/api/rest/apiService";
import {PlayerState} from "@/datatypes/PlayerState";
import QueuedSongSkeleton from "@/components/skeletons/QueuedSongSkeleton";

interface PlayerControlProps {
    toggleQueue: () => void;
    track: TrackDto | null;
    positionInSeconds: number;
    guildId: number;
    state: PlayerState;
    loading: boolean;
}

const PlayerControl: React.FC<PlayerControlProps> = ({
                                                         toggleQueue,
                                                         track,
                                                         positionInSeconds,
                                                         guildId,
                                                         state,
                                                         loading
                                                     }) => {
    const [sliderValue, setSliderValue] = useState(positionInSeconds);
    const [currentTrack, setCurrentTrack] = useState(track);
    const [playerState, setPlayerState] = useState(state);
    const [sliderLoading, setSliderLoading] = useState(false);
    const [skipLoading, setSkipLoading] = useState(false);
    const [rewindLoading, setRewindLoading] = useState(false);
    const [pauseLoading, setPauseLoading] = useState(false);
    const [stopLoading, setStopLoading] = useState(false);
    const [leaveLoading, setLeaveLoading] = useState(false);

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
        setSliderLoading(true);
        await RequestSeek(guildId, value[0]);
        setSliderLoading(false);
    };

    return (
        <div
            className="flex flex-row justify-center items-center ml-2 mb-0.5 text-center border-t-2 mr-2 border-black mt-1 pt-2 dark:border-white">
            {loading ? <div className="w-1/3"><QueuedSongSkeleton/></div> :
                <div className="flex flex-row space-x-3 w-1/3">
                    <img
                        className="h-[60px] w-[60px] rounded-xl object-cover"
                        src={currentTrack?.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                        onError={({currentTarget}) => {
                            currentTarget.onerror = null; // prevents looping
                            currentTarget.src = "/bluray-disc-icon.svg";
                        }}
                        alt='track icon'
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
            }
            <div className="felx flex-row w-1/3">
                <div className="flex items-center justify-center">
                    {stopLoading ?
                        <Loader2 className="animate-spin h-9 w-9 text-primary mr-2"/>
                        :
                        <DefaultButton disabled={playerState == PlayerState.Destroyed} tooltipText="Stop" onClick={async () => {
                            setStopLoading(true)
                            await RequestStop(guildId)
                            setStopLoading(false)
                        }} className="mr-2">
                            <Stop className="w-8 h-8"/>
                        </DefaultButton>
                    }
                    {rewindLoading ?
                        <Loader2 className="animate-spin h-10 w-10 text-primary mx-1"/>
                        :
                        <DefaultButton disabled={currentTrack === null} tooltipText="Rewind" onClick={async () => {
                            setRewindLoading(true)
                            await RequestRewind(guildId)
                            setRewindLoading(false)
                        }}>
                            <ChevronLeft className="w-10 h-10"/>
                        </DefaultButton>
                    }
                    {pauseLoading ?
                        <Loader2 className="animate-spin h-10 w-10 text-primary"/>
                        :
                        <DefaultButton disabled={currentTrack === null} tooltipText="Play/Pause" onClick={
                            async () => {
                                setPauseLoading(true);
                                playerState === PlayerState.Playing ? await RequestPause(guildId) : await RequestResume(guildId);
                                setPauseLoading(false);
                            }
                        }>
                            {playerState === PlayerState.Playing ? <PauseIcon className="h-8 w-8"/> :
                                <TriangleIcon className="h-8 w-8 rotate-90"/>}
                        </DefaultButton>
                    }
                    {skipLoading ?
                        <Loader2 className="animate-spin h-10 w-10 text-primary mx-1"/>
                        :
                        <DefaultButton disabled={currentTrack === null} tooltipText="Skip" onClick={async () => {
                            setSkipLoading(true)
                            await RequestSkip(guildId)
                            setSkipLoading(false)
                        }}>
                            <ChevronRight className="w-10 h-10"/>
                        </DefaultButton>
                    }
                    {leaveLoading ?
                        <Loader2 className="animate-spin h-9 w-9 text-primary ml-2"/>
                        :
                        <DefaultButton disabled={playerState == PlayerState.Destroyed} tooltipText="Leave" onClick={async () => {
                            setLeaveLoading(true)
                            await RequestLeave(guildId)
                            setLeaveLoading(false)
                        }} className="ml-2">
                            <Door className="w-8 h-8"/>
                        </DefaultButton>
                    }
                </div>
                <div className="flex justify-center">
                    <div className="flex items-center justify-center w-11/12 pb-2 select-none">
                        {formatDuration(sliderValue ?? 0)}
                        <div className="w-full min-w-56 px-4">
                            <Slider
                                disabled={sliderLoading || currentTrack === null}
                                loading={sliderLoading}
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