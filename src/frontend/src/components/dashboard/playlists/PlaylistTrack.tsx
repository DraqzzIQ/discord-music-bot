import React from "react";
import {TrackDto} from "@/dtos/TrackDto";
import {RequestDeleteTrack, RequestPlay,} from "@/api/rest/apiService";
import {formatDuration} from "@/lib/utils";
import {DotsVerticalIcon} from "@radix-ui/react-icons";
import {
    ContextMenu,
    ContextMenuContent,
    ContextMenuItem,
    ContextMenuSeparator,
    ContextMenuTrigger
} from "@/components/ui/context-menu";
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger} from "@/components/ui/dropdown-menu";
import ErrorAlert from "@/components/dashboard/ErrorAlert";
import AddToPlaylistPopover from "@/components/dashboard/AddToPlaylistPopover";
import DefaultButton from "@/components/DefaultButton";
import {PlusIcon} from "lucide-react";

interface PlaylistTrackProps {
    track: TrackDto;
    guildId: number;
    onRemove: (trackId: string) => void;
    playlistId: string;
}

export default function PlaylistTrack({track, guildId, onRemove, playlistId}: PlaylistTrackProps) {
    const [errorPlaying, setErrorPlaying] = React.useState(false);

    const handleRemove = async () => {
        onRemove(track.id);
        await RequestDeleteTrack(guildId, playlistId, track.id);
    }

    const handleAddToQueue = async () => {
        const response = await RequestPlay(guildId, {
            isPlaylist: false,
            shouldPlay: false,
            encodedTrack: track.encodedTrack
        });
        if (!response) {
            setErrorPlaying(true);
        }
    }

    const handlePlay = async () => {
        const response = await RequestPlay(guildId, {
            isPlaylist: false,
            shouldPlay: true,
            encodedTrack: track.encodedTrack
        });
        if (!response) {
            setErrorPlaying(true);
        }
    }

    return (
        <>
            {errorPlaying && <ErrorAlert title="No voice channel"
                                         error="Please connect to a voice channel the bot can access and try again."
                                         onDismiss={() => setErrorPlaying(false)}/>}
            <DropdownMenu>
                <ContextMenu>
                    <ContextMenuTrigger>
                        <div className="flex justify-between w-full items-center cursor-move">
                            <div className="flex flex-row space-x-1.5 w-[calc(100%-150px)]">
                                <img
                                    className={`h-[55px] w-[55px] rounded-xl object-cover ${track.artworkUrl ? '' : 'dark:invert'}`}
                                    src={track.artworkUrl ?? '/bluray-disc-icon.svg'}
                                    onError={({currentTarget}) => {
                                        currentTarget.onerror = null; // prevents looping
                                        currentTarget.src = "/bluray-disc-icon.svg";
                                        currentTarget.className = "h-[55px] w-[55px] rounded-xl object-cover dark:invert";
                                    }}
                                    alt='track icon'/>
                                <div className="space-y-1.5 w-full">
                                    <div className="h-6 truncate">
                                        <a href={track.url} target="_blank" rel="noreferrer"
                                           className="hover:underline font-semibold text-lg text-black dark:text-white">
                                            {track.title}
                                        </a>
                                    </div>
                                    <div className="text-gray-400 truncate">
                                        {track.author} • {formatDuration(track.durationInSeconds)}
                                    </div>
                                </div>
                            </div>
                            <div className="flex items-center">
                                <AddToPlaylistPopover child={
                                    <DefaultButton tooltipText={"Add to playlist"}>
                                        <PlusIcon size="28"/>
                                    </DefaultButton>
                                } guildId={guildId} track={track}>
                                </AddToPlaylistPopover>
                                <DropdownMenuTrigger>
                                    <div className="p-2 mr-1 hover:scale-110 cursor-pointer">
                                        <DotsVerticalIcon className="h-5 w-5"/>
                                    </div>
                                </DropdownMenuTrigger>
                            </div>
                        </div>
                    </ContextMenuTrigger>
                    <ContextMenuContent>
                        <ContextMenuItem onClick={handlePlay}>Play</ContextMenuItem>
                        <ContextMenuItem onClick={handleAddToQueue}>Add to queue</ContextMenuItem>
                        <ContextMenuSeparator/>
                        <ContextMenuItem onClick={handleRemove}>Remove</ContextMenuItem>
                    </ContextMenuContent>
                </ContextMenu>
                <DropdownMenuContent>
                    <DropdownMenuItem onClick={handlePlay}>Play</DropdownMenuItem>
                    <DropdownMenuItem onClick={handleAddToQueue}>Add to queue</DropdownMenuItem>
                    <ContextMenuSeparator/>
                    <DropdownMenuItem onClick={handleRemove}>Remove</DropdownMenuItem>
                </DropdownMenuContent>
            </DropdownMenu>
        </>
    );
};