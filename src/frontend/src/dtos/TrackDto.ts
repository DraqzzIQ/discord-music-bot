export interface TrackDto {
    title: string;
    author: string;
    durationInSeconds: number;
    thumbnailUrl?: string | null;
    url?: string;
    encodedTrack?: string;
}