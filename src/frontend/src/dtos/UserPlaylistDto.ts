import {TrackDto} from "@/dtos/TrackDto";

export default interface UserPlaylistDto {
    id: string,
    name: string,
    ownerUsername: string,
    ownerAvatarUrl: string,
    isPublic: boolean,
    isOwn: boolean,
    tracks: TrackDto[],
}