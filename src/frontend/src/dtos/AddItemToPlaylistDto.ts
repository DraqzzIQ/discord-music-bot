export default interface AddItemToPlaylistDto {
    id: string;
    name: string;
    containsItem: boolean;
    previewUrls: string[];
}