import {TrackDto} from "@/dtos/TrackDto";

export interface PlaylistDto {
    title: string;
    author?: string;
    url?: string;
    artworkUrl?: string | null;
    trackCount?: number;
    selectedTrack?: TrackDto;
    encodedTracks?: string[];
}