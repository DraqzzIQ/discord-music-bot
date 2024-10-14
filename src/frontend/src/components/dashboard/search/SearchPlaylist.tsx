import {TrackDto} from "@/dtos/TrackDto";
import {PlaylistDto} from "@/dtos/PlaylistDto";
import {Playlist} from "@phosphor-icons/react";

export interface SearchPlaylistProps {
    playlist: PlaylistDto
}

export default function SearchPlaylist({playlist}: SearchPlaylistProps) {
    return (
        <div className="flex flex-row space-x-1.5 min-w-0">
            <img className={`h-[55px] w-[55px] rounded-xl object-cover ${(playlist.artworkUrl || playlist.selectedTrack?.thumbnailUrl) ? '' : 'dark:invert'}`}
                 src={playlist.artworkUrl ?? playlist.selectedTrack?.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                 alt=''/>
            <div className="space-y-4 flex-grow">
                <div className="h-4">
                    <a href={playlist.url ?? playlist?.selectedTrack?.url} target="_blank" rel="noreferrer"
                       className="hover:underline font-semibold text-lg block truncate">
                        {playlist.title}
                    </a>
                </div>
                <div className="text-gray-400 truncate">{playlist.author ?? playlist.selectedTrack?.author}</div>
            </div>
        </div>
    )
}