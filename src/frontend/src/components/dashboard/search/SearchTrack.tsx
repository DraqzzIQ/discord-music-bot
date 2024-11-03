import {TrackDto} from "@/dtos/TrackDto";
import {ListPlusIcon, Loader2, PlayIcon, PlusIcon} from "lucide-react";
import DefaultButton from "@/components/DefaultButton";
import {formatDuration} from "@/lib/utils";
import {ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuTrigger} from "@/components/ui/context-menu";
import {RequestPlay} from "@/api/rest/apiService";
import {useState} from "react";
import AddToPlaylistPopover from "@/components/dashboard/AddToPlaylistPopover";

export interface SearchTrackProps {
    track: TrackDto,
    guildId: number,
    setOnErrorPlaying: () => void
}

export default function SearchTrack({track, guildId, setOnErrorPlaying}: SearchTrackProps) {
    const [playLoading, setPlayLoading] = useState(false);
    const [addToQueueLoading, setAddToQueueLoading] = useState(false);
    const [addToPlaylistLoading, setAddToPlaylistLoading] = useState(false);

    const handlePlay = async () => {
        setPlayLoading(true);
        let success: boolean = await RequestPlay(guildId, {
            isPlaylist: false,
            shouldPlay: true,
            encodedTrack: track.encodedTrack,
        });
        if (!success)
            setOnErrorPlaying();
        setPlayLoading(false);
    }

    const handleAddToQueue = async () => {
        setAddToQueueLoading(true);
        let success: boolean = await RequestPlay(guildId, {
            isPlaylist: false,
            shouldPlay: false,
            encodedTrack: track.encodedTrack,
        });
        if (!success)
            setOnErrorPlaying();
        setAddToQueueLoading(false);
    }

    return (
        <ContextMenu>
            <ContextMenuTrigger>
                <div className="flex justify-between items-center w-full my-3">
                    <div className="w-[calc(100%-150px)] flex space-x-1.5">
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
                            <div
                                className="text-gray-400 truncate">{track.author} • {formatDuration(track.durationInSeconds)}</div>
                        </div>
                    </div>
                    <div className="pr-5 flex space-x-4">
                        <AddToPlaylistPopover child={
                            <DefaultButton tooltipText={"Add to playlist"}>
                                <PlusIcon size="28"/>
                            </DefaultButton>
                        } guildId={guildId} track={track}>
                        </AddToPlaylistPopover>
                        <DefaultButton tooltipText="Play" onClick={handlePlay} disabled={playLoading}>
                            {playLoading ?
                                <Loader2 className="animate-spin h-[22px] w-[22px] text-primary"/>
                                :
                                <PlayIcon size="22"/>
                            }
                        </DefaultButton>
                        <DefaultButton tooltipText="Add to queue" onClick={handleAddToQueue}
                                       disabled={addToQueueLoading}>
                            {addToQueueLoading ?
                                <Loader2 className="animate-spin h-[22px] w-[22px] text-primary"/>
                                :
                                <ListPlusIcon size="22"/>
                            }
                        </DefaultButton>
                    </div>
                </div>
            </ContextMenuTrigger>
            <ContextMenuContent>
                <ContextMenuItem onClick={handlePlay}>Play</ContextMenuItem>
                <ContextMenuItem onClick={handleAddToQueue}>Add to queue</ContextMenuItem>
            </ContextMenuContent>
        </ContextMenu>
    )
}