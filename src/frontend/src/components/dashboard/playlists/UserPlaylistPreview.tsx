import UserPlaylistPreviewDto from "@/dtos/UserPlaylistPreviewDto";
import {LockClosedIcon} from "@radix-ui/react-icons";
import {ListPlusIcon, Loader2, PenIcon, Pin, PinIcon, PlayIcon, Shuffle, TrashIcon} from "lucide-react";
import React, {useState} from "react";
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";
import {
    ContextMenu,
    ContextMenuContent,
    ContextMenuItem,
    ContextMenuSeparator,
    ContextMenuTrigger
} from "@/components/ui/context-menu";
import {RequestDeletePlaylist, RequestPlayPlaylist, RequestShufflePlaylist} from "@/api/rest/apiService";
import ErrorAlert from "@/components/dashboard/ErrorAlert";
import PlaylistDialog from "@/components/dashboard/playlists/PlaylistDialog";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger
} from "@/components/ui/alert-dialog";

interface UserPlaylistPreviewProps {
    playlist: UserPlaylistPreviewDto,
    onPinChange: (playlistId: string, isPinned: boolean) => void,
    guildId: number,
    refresh: () => Promise<void>,
    selectPlaylist: (playlist: UserPlaylistPreviewDto) => Promise<void>,
}

export default function UserPlaylistPreview({
                                                playlist,
                                                onPinChange,
                                                guildId,
                                                refresh,
                                                selectPlaylist
                                            }: UserPlaylistPreviewProps) {
    const [isPinned, setIsPinned] = useState<boolean>(playlist.isPinned);
    const [error, setError] = useState<string | null>(null);
    const [errorTitle, setErrorTitle] = useState<string | null>(null);
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(false);

    const onPinClick = async () => {
        setIsPinned(!isPinned);
        onPinChange(playlist.id, !isPinned);
    }

    const onDelete = async () => {
        setIsLoading(true);
        await RequestDeletePlaylist(guildId, playlist.id);
        setIsLoading(false);
        await refresh();
    }

    const onPlay = async (shouldPlay: boolean) => {
        setIsLoading(true);
        const response: any = await RequestPlayPlaylist(guildId, playlist.id, shouldPlay);
        if (response.replace(/"/g, '') == "Player not found") {
            setError("Please connect to a voice channel the bot can access and try again.");
            setErrorTitle("No voice channel");
        } else if (response != "") {
            setError(response.replace(/"/g, ''));
            setErrorTitle(shouldPlay ? "Failed to play playlist" : "Failed to add playlist to queue");
        }
        setIsLoading(false);
    }

    const onShuffle = async () => {
        setIsLoading(true);
        await RequestShufflePlaylist(guildId, playlist.id);
        setIsLoading(false);
        await refresh();
    }

    const onAlertDismiss = () => {
        setError(null);
    }

    const onClick = async () => {
        await selectPlaylist(playlist);
    }

    return (
        <div>
            {isLoading ? (
                <div className="h-40 w-40 flex justify-center items-center">
                    <Loader2 className="animate-spin rounded-full h-20 w-20"/>
                </div>
            ) : (
                <>
                    {error && errorTitle &&
                        <ErrorAlert title={errorTitle} error={error} onDismiss={onAlertDismiss}/>
                    }
                    <AlertDialog>
                        <ContextMenu modal={isMenuOpen} onOpenChange={(isOpen) => setIsMenuOpen(isOpen)}>
                            <ContextMenuTrigger>
                                <div className="relative group">
                                    <div onClick={onClick}
                                         className={`h-[160px] w-[160px] border-2 rounded-xl grid ${playlist.previewUrls.length > 0 ? 'grid-cols-2' : ''} overflow-hidden`}>
                                        {playlist.previewUrls.length === 0 &&
                                            <img src="/bluray-disc-icon.svg" alt={playlist.name}
                                                 className="w-[160px] h-[160px] p-[20px] dark:invert"/>}
                                        <img src={playlist.previewUrls[0] ?? "/transparent-square.svg"}
                                             alt={playlist.name}
                                             className="w-[80px] h-[80px] object-cover"/>
                                        <img src={playlist.previewUrls[1] ?? "/transparent-square.svg"}
                                             alt={playlist.name}
                                             className="w-[80px] h-[80px] object-cover"/>
                                        <img src={playlist.previewUrls[2] ?? "/transparent-square.svg"}
                                             alt={playlist.name}
                                             className="w-[80px] h-[80px] object-cover"/>
                                        <img src={playlist.previewUrls[3] ?? "/transparent-square.svg"}
                                             alt={playlist.name}
                                             className="w-[80px] h-[80px] object-cover"/>
                                    </div>
                                    <div
                                        className={`absolute top-2 right-2 ${isPinned ? '' : 'opacity-0 group-hover:opacity-100'} transition-opacity cursor-pointer`}
                                        onClick={onPinClick}>
                                        <PinIcon className={`dark:invert ${isPinned ? 'text-black' : ''}`}/>
                                    </div>
                                    <div>
                                        <div className="flex items-center">
                                            <div className="font-semibold w-[160px] truncate">{playlist.name}</div>
                                            <div className="ml-2">{playlist.isPublic ? <div/> : <LockClosedIcon/>}</div>
                                        </div>
                                        <div className="flex items-center">
                                            <div className="max-w-[160px] truncate">by {playlist.ownerUsername}</div>
                                            <Avatar className="h-6 w-6 ml-2">
                                                <AvatarImage src={playlist.ownerAvatarUrl}/>
                                                <AvatarFallback>{playlist.ownerUsername[0]}</AvatarFallback>
                                            </Avatar>
                                        </div>
                                    </div>
                                </div>
                            </ContextMenuTrigger>
                            <ContextMenuContent>
                                <ContextMenuItem className="h-9" onClick={() => onPlay(true)}>
                                    <div className="flex items-center">
                                        <PlayIcon className="h-5 w-5 mr-2"/>
                                        Play
                                    </div>
                                </ContextMenuItem>
                                <ContextMenuItem className="h-9" onClick={() => onPlay(false)}>
                                    <div className="flex items-center">
                                        <ListPlusIcon className="h-5 w-5 mr-2"/>
                                        Add to queue
                                    </div>
                                </ContextMenuItem>
                                <ContextMenuSeparator/>
                                <ContextMenuItem className="h-9" onClick={onShuffle}
                                                 disabled={!(playlist.isOwn || playlist.isPublic)}>
                                    <div className="flex items-center">
                                        <Shuffle className="h-5 w-5 mr-2"/>
                                        Shuffle
                                    </div>
                                </ContextMenuItem>
                                <ContextMenuSeparator/>
                                <PlaylistDialog onClose={() => setIsMenuOpen(false)} createType={false}
                                                guildId={guildId}
                                                refresh={refresh} playlist={playlist} child={
                                    <ContextMenuItem className="h-9" disabled={!playlist.isOwn}
                                                     onSelect={(e) => e.preventDefault()}>
                                        <div className="flex items-center">
                                            <PenIcon className="h-5 w-5 mr-2"/>
                                            Edit
                                        </div>
                                    </ContextMenuItem>
                                }
                                />
                                <ContextMenuSeparator/>
                                <AlertDialogTrigger asChild>
                                    <ContextMenuItem className="h-9" disabled={!playlist.isOwn}>
                                        <div className="flex items-center">
                                            <TrashIcon className="h-5 w-5 mr-2"/>
                                            Delete
                                        </div>
                                    </ContextMenuItem>
                                </AlertDialogTrigger>
                                <ContextMenuSeparator className="h-0.5"/>
                                <ContextMenuItem className="h-9" onClick={onPinClick}>
                                    <div className="flex items-center">
                                        <Pin className="h-5 w-5 mr-2"/>
                                        {isPinned ? 'Unpin' : 'Pin'}
                                    </div>
                                </ContextMenuItem>
                            </ContextMenuContent>
                        </ContextMenu>
                        <AlertDialogContent>
                            <AlertDialogHeader>
                                <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                                <AlertDialogDescription>
                                    This cannot be undone.
                                </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                                <AlertDialogCancel>Cancel</AlertDialogCancel>
                                <AlertDialogAction onClick={onDelete}>Delete</AlertDialogAction>
                            </AlertDialogFooter>
                        </AlertDialogContent>
                    </AlertDialog>
                </>
            )}
        </div>
    );
}