import {type ClassValue, clsx} from "clsx";
import {twMerge} from "tailwind-merge";
import {exact} from "prop-types";

export function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

export function formatDuration(duration: number) {
    const hours = Math.floor(duration / 3600);
    const minutes = Math.floor(duration % 3600 / 60);
    const seconds = duration % 60;
    if (hours > 0) {
        return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    }
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
}