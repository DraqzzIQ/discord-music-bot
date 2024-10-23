import {TrackDto} from "@/dtos/TrackDto";
import {PlayerState} from "@/datatypes/PlayerState";

export interface PlayerUpdatedDto {
    updateQueue: boolean;
    currentTrack: TrackDto;
    positionInSeconds: number;
    queue: TrackDto[];
    state: PlayerState;
}