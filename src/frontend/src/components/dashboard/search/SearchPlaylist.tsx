import {PlaylistDto} from "@/dtos/PlaylistDto";
import {ListPlusIcon, Loader2, PlayIcon, PlusIcon} from "lucide-react";
import DefaultButton from "@/components/DefaultButton";
import {ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuTrigger} from "@/components/ui/context-menu";
import {RequestPlay} from "@/api/rest/apiService";
import {useState} from "react";
import AddToPlaylistPopover from "@/components/dashboard/AddToPlaylistPopover";

export interface SearchPlaylistProps {
    playlist: PlaylistDto,
    guildId: number,
    setOnErrorPlaying: () => void
}

export default function SearchPlaylist({playlist, guildId, setOnErrorPlaying}: SearchPlaylistProps) {
    const [playLoading, setPlayLoading] = useState(false);
    const [addToQueueLoading, setAddToQueueLoading] = useState(false);

    const handlePlay = async () => {
        setPlayLoading(true);
        let success: boolean = await RequestPlay(guildId, {
            isPlaylist: true,
            shouldPlay: true,
            playlistUrl: playlist.url,
            encodedPlaylistTracks: playlist.encodedTracks,
        });
        if (!success)
            setOnErrorPlaying();
        setPlayLoading(false);
    }

    const handleAddToQueue = async () => {
        setAddToQueueLoading(true);
        let success: boolean = await RequestPlay(guildId, {
            isPlaylist: true,
            shouldPlay: false,
            playlistUrl: playlist.url,
            encodedPlaylistTracks: playlist.encodedTracks,
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
                            className={`h-[55px] w-[55px] rounded-xl object-cover ${playlist.artworkUrl ? '' : 'dark:invert'}`}
                            src={playlist.artworkUrl ?? '/bluray-disc-icon.svg'}
                            onError={({currentTarget}) => {
                                currentTarget.onerror = null; // prevents looping
                                currentTarget.src = "/bluray-disc-icon.svg";
                                currentTarget.className = "h-[55px] w-[55px] rounded-xl object-cover dark:invert";
                            }}
                            alt='track icon'/>
                        <div className="space-y-1.5 w-full">
                            <div className="h-6 truncate">
                                <a href={playlist.url ?? playlist.selectedTrack?.url} target="_blank" rel="noreferrer"
                                   className="hover:underline font-semibold text-lg text-black dark:text-white">
                                    {playlist.title}
                                </a>
                            </div>
                            <div className="text-gray-400 truncate">
                                {playlist.author ?? playlist.selectedTrack?.author} {playlist.trackCount ? ("• " + playlist.trackCount + " songs") : ""}
                            </div>
                        </div>
                    </div>
                    <div className="pr-5 flex space-x-4">
                        <AddToPlaylistPopover child={
                            <DefaultButton tooltipText={"Add to playlist"}>
                                <PlusIcon size="28"/>
                            </DefaultButton>
                        } guildId={guildId} playlist={playlist}>
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
                <ContextMenuItem>Add to queue</ContextMenuItem>
            </ContextMenuContent>
        </ContextMenu>
    )
}