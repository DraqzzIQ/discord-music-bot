"use client";

import PlayerControl from "@/components/player/PlayerControl";
import React, {useEffect} from "react";
import {socketService} from "@/api/signalr/socket";
import {PlayerUpdatedDto} from "@/dtos/PlayerUpdatedDto";
import {TrackDto} from "@/dtos/TrackDto";
import QueuedSong from "@/components/queue/QueuedSong";
import {PlayerState} from "@/datatypes/PlayerState";
import SongQueue from "@/components/queue/SongQueue";
import DashboardTabs from "@/components/dashboard/DashboardTabs";
import QueuedSongSkeleton from "@/components/skeletons/QueuedSongSkeleton";

export default function Dash({params} : {params: {guildId: number}}) {
    const [showQueue, setShowQueue] = React.useState(true);
    const [track, setTrack] = React.useState<TrackDto | null>(null);
    const [positionInSeconds, setPositionInSeconds] = React.useState<number>(0);
    const [queue, setQueue] = React.useState<TrackDto[]>([]);
    const [state, setState] = React.useState<PlayerState>(PlayerState.Destroyed);
    const [loading, setLoading] = React.useState(true);
    
    const handleReorder = async (sourceIndex: number, destinationIndex: number) => {
        const reorderedQueue = Array.from(queue);
        const [movedItem] = reorderedQueue.splice(sourceIndex, 1);
        reorderedQueue.splice(destinationIndex, 0, movedItem);
        setQueue(reorderedQueue);
    }

    const handleRemove = (index: number) => {
        setQueue(prevQueue => prevQueue.filter((_, i) => i !== index));
    }
    
    const handleSkipTo = (index: number) => {
        if(index === 0) return;
        setQueue(prevQueue => {
            const newQueue = Array.from(prevQueue);
            newQueue.splice(0, index + 1);
            return newQueue;
        });
    }

    useEffect(() => {
        const handleUpdatePlayer = (payload: PlayerUpdatedDto) => {
            let shouldUpdateQueue = payload.updateQueue;
            setTrack(payload.currentTrack);
            setPositionInSeconds(payload.positionInSeconds);
            if (shouldUpdateQueue) setQueue(payload.queue);
            setState(payload.state);
            setLoading(false);
        };
        
        const handleUpdatePosition = (position: number) => {
            setPositionInSeconds(position);
        }

        const registerEventHandlers = () => {
            socketService.registerOnServerEvents('UpdatePlayer', handleUpdatePlayer);
            socketService.registerOnServerEvents('UpdatePosition', handleUpdatePosition);
        };

        const fetchInitialData = async () => {
            await socketService.invokeMethod('SubscribeToPlayer', params.guildId);
            await socketService.invokeMethod('GetPlayerStatus', null);
        };

        const setupConnectionCallbacks = () => {
            socketService.registerOnConnectedCallback(fetchInitialData);
            socketService.registerOnReconnectedCallback(fetchInitialData);
        };

        registerEventHandlers();
        setupConnectionCallbacks();

        return () => {
            socketService.stopConnection();
        };
    }, [params.guildId]);

    return (
        <div className="flex flex-col h-screen">
            <div className="flex flex-row flex-grow overflow-hidden">
                <DashboardTabs guildId={params.guildId} track={track}/>
                {showQueue && <SongQueue guildId={params.guildId} onReorder={handleReorder}>
                    {loading ? (
                        Array.from({length: 20}).map((_, index) => (
                            <QueuedSongSkeleton key={index}/>
                        ))
                    ): queue.length === 0 ? (
                        <div className="font-semibold text-center">Queue is empty</div>
                    ) : (
                        queue.map((track, index) => (
                            <QueuedSong key={index} track={track} guildId={params.guildId} index={index} onRemove={handleRemove} onMoveToTop={handleReorder} onSkipTo={handleSkipTo}/>
                        ))
                    )}
                </SongQueue>}
            </div>
            <PlayerControl toggleQueue={() => setShowQueue(!showQueue)}
                           track={track}
                           positionInSeconds={positionInSeconds}
                           guildId={params.guildId}
                           state={state}
                           loading={loading}
            />
        </div>
    );
};