export interface PlayRequestDto {
    isPlaylist: boolean;
    shouldPlay: boolean;
    encodedTrack?: string;
    playlistUrl?: string;
    encodedPlaylistTracks?: string[];
}