export default interface UserPlaylistPreviewDto {
    id: string,
    name: string,
    ownerUsername: string,
    ownerAvatarUrl: string,
    isPublic: boolean,
    isOwn: boolean,
    isPinned: boolean,
    previewUrls: string[],
}