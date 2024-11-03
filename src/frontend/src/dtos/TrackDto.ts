export interface TrackDto {
    id: string
    title: string;
    author: string;
    durationInSeconds: number;
    artworkUrl?: string | null;
    url?: string;
    encodedTrack?: string;
}