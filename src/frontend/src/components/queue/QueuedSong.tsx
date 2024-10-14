import React from "react";
import {TrackDto} from "@/dtos/TrackDto";
import {XIcon} from "lucide-react";
import {RequestRemove} from "@/api/rest/apiService";

interface QueuedSongProps {
    index: number;
    track: TrackDto;
    guildId: number;
    onRemove: (index: number) => void;
}

const QueuedSong: React.FC<QueuedSongProps> = ({index, track, guildId, onRemove}) => {
    const handleRemove = async () => {
        onRemove(index);
        await RequestRemove(guildId, index);
    }

    return (
        <div className="flex justify-between w-full items-center">
            <div className="flex flex-row space-x-1.5 min-w-0">
                <img className={`h-[55px] w-[55px] rounded-xl object-cover ${track?.thumbnailUrl ? '' : 'dark:invert'}`}
                     src={track?.thumbnailUrl ?? '/bluray-disc-icon.svg'}
                     alt=''/>
                <div className="space-y-4 flex-grow">
                    <div className="h-4">
                        <a href={track?.url} target="_blank" rel="noreferrer"
                           className="hover:underline font-semibold text-lg block truncate">
                            {track?.title}
                        </a>
                    </div>
                    <div className="text-gray-400 truncate">{track?.author}</div>
                </div>
            </div>
            <div className="flex-shrink-0">
                <XIcon className="h-5 w-5 cursor-pointer hover:scale-110"
                       onClick={handleRemove}/>
            </div>
        </div>
    );
};

export default QueuedSong;