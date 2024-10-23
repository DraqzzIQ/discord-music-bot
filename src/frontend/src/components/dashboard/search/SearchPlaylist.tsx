import {PlaylistDto} from "@/dtos/PlaylistDto";
import {Queue} from "@phosphor-icons/react";
import {PlayIcon} from "lucide-react";
import DefaultButton from "@/components/DefaultButton";
import {ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuTrigger} from "@/components/ui/context-menu";
import {RequestPlay} from "@/api/rest/apiService";

export interface SearchPlaylistProps {
    playlist: PlaylistDto,
    guildId: number
}

export default function SearchPlaylist({playlist, guildId}: SearchPlaylistProps) {
    const handlePlay = async () => {
        await RequestPlay(guildId, {
            isPlaylist: true,
            shouldPlay: true,
            playlistUrl: playlist.url,
            encodedPlaylistTracks: playlist.encodedTracks,
        });
    }
    
    const handleAddToQueue = async () => {
        await RequestPlay(guildId, {
            isPlaylist: true,
            shouldPlay: false,
            playlistUrl: playlist.url,
            encodedPlaylistTracks: playlist.encodedTracks,
        });
    }
    
    
    return (
        <ContextMenu>
            <ContextMenuTrigger>
                <div className="flex justify-between items-center w-full my-3">
                    <div className="w-[calc(100%-150px)] flex space-x-1.5">
                        <img
                            className="h-[55px] w-[55px] rounded-xl object-cover"
                            src={playlist.artworkUrl ?? playlist.selectedTrack?.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                            onError={({currentTarget}) => {
                                currentTarget.onerror = null; // prevents looping
                                currentTarget.src = "/bluray-disc-icon.svg";
                            }}
                            alt='playlist icon'/>
                        <div className="space-y-1.5 w-full">
                            <div className="h-6 truncate">
                                <a href={playlist.url ?? playlist.selectedTrack?.url} target="_blank" rel="noreferrer"
                                   className="hover:underline font-semibold text-lg">
                                    {playlist.title}
                                </a>
                            </div>
                            <div className="text-gray-400 truncate">
                                {playlist.author ?? playlist.selectedTrack?.author} {playlist.trackCount ? ("• " + playlist.trackCount + " songs") : ""}
                            </div>
                        </div>
                    </div>
                    <div className="pr-5 flex space-x-4">
                        <DefaultButton tooltipText="Play" onClick={handlePlay}>
                            <PlayIcon size="22"/>
                        </DefaultButton>
                        <DefaultButton tooltipText="Add to queue" onClick={handleAddToQueue}>
                            <Queue size="22"/>
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