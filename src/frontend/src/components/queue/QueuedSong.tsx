import React from "react";
import {TrackDto} from "@/dtos/TrackDto";
import {RequestRemove, RequestReorder, RequestSkip} from "@/api/rest/apiService";
import {formatDuration} from "@/lib/utils";
import {DotsVerticalIcon} from "@radix-ui/react-icons";
import {ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuTrigger} from "@/components/ui/context-menu";
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger} from "@/components/ui/dropdown-menu";

interface QueuedSongProps {
    index: number;
    track: TrackDto;
    guildId: number;
    onRemove: (index: number) => void;
    onMoveToTop: (index: number, destIndex: number) => void;
    onSkipTo: (index: number) => void;
}

const QueuedSong: React.FC<QueuedSongProps> = ({index, track, guildId, onRemove, onMoveToTop, onSkipTo}) => {
    const handleRemove = async () => {
        onRemove(index);
        await RequestRemove(guildId, index);
    }

    const handleMoveToTop = async () => {
        onMoveToTop(index, 0);
        await RequestReorder(guildId, index, 0);
    }

    const handleSkipTo = async () => {
        onSkipTo(index);
        await RequestSkip(guildId, index);
    }

    return (
        <DropdownMenu>
            <ContextMenu>
                <ContextMenuTrigger>
                    <div className="flex justify-between w-full items-center cursor-move">
                        <div className="flex flex-row space-x-1.5 w-[calc(100%-110px)]">
                            <img
                                className="h-[55px] w-[55px] rounded-xl object-cover"
                                src={track.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                                onError={({currentTarget}) => {
                                    currentTarget.onerror = null; // prevents looping
                                    currentTarget.src = "/bluray-disc-icon.svg";
                                }}
                                alt='track icon'/>
                            <div className="space-y-1.5 w-full">
                                <div className="h-6 truncate">
                                    <a href={track.url} target="_blank" rel="noreferrer"
                                       className="hover:underline font-semibold text-lg">
                                        {track.title}
                                    </a>
                                </div>
                                <div className="text-gray-400 truncate">
                                    {track.author} • {formatDuration(track.durationInSeconds)}
                                </div>
                            </div>
                        </div>
                        <DropdownMenuTrigger>
                            <div className="p-2 mr-1 hover:scale-110 cursor-pointer">
                                <DotsVerticalIcon className="h-5 w-5"/>
                            </div>
                        </DropdownMenuTrigger>
                    </div>
                </ContextMenuTrigger>
                <ContextMenuContent>
                    <ContextMenuItem onClick={handleMoveToTop}>Move to top</ContextMenuItem>
                    <ContextMenuItem onClick={handleSkipTo}>Skip to</ContextMenuItem>
                    <ContextMenuItem onClick={handleRemove}>Remove</ContextMenuItem>
                </ContextMenuContent>
            </ContextMenu>
            <DropdownMenuContent>
                <DropdownMenuItem onClick={handleMoveToTop}>Move to top</DropdownMenuItem>
                <DropdownMenuItem onClick={handleSkipTo}>Skip to</DropdownMenuItem>
                <DropdownMenuItem onClick={handleRemove}>Remove</DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
};

export default QueuedSong;