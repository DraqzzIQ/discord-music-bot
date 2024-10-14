import {TrackDto} from "@/dtos/TrackDto";
import {PlaylistDto} from "@/dtos/PlaylistDto";

export interface SearchResponseDto {
    tracks: TrackDto[];
    playlists: PlaylistDto[];
    albums: PlaylistDto[];
    searchMode?: string;
}