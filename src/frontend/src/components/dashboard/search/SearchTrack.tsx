import {TrackDto} from "@/dtos/TrackDto";

export interface SearchTrackProps {
    track: TrackDto
}

export default function SearchTrack({track}: SearchTrackProps) {
    return (
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
    )
}