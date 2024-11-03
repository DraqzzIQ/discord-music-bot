import UserPlaylistDto from "@/dtos/UserPlaylistDto";
import React, {useEffect, useState} from "react";
import {LockClosedIcon} from "@radix-ui/react-icons";
import {ArrowLeftIcon, ListPlusIcon, Loader2, PenIcon, PlayIcon, RefreshCwIcon, TrashIcon} from "lucide-react";
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";
import PlaylistDialog from "@/components/dashboard/playlists/PlaylistDialog";
import UserPlaylistSkeleton from "@/components/skeletons/UserPlaylistSkeleton";
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
import {
    RequestDeletePlaylist,
    RequestDeleteTrack,
    RequestPlayPlaylist,
    RequestReorderPlaylist
} from "@/api/rest/apiService";
import {ScrollArea} from "@/components/ui/scroll-area";
import PlaylistTrack from "@/components/dashboard/playlists/PlaylistTrack";
import {DragDropContext, Draggable, Droppable} from "@hello-pangea/dnd";
import {formatDuration} from "@/lib/utils";
import {TrackDto} from "@/dtos/TrackDto";
import ErrorAlert from "@/components/dashboard/ErrorAlert";

interface UserPlaylistProps {
    playlist: UserPlaylistDto | null;
    goBack: () => void;
    guildId: number;
    onRefresh: () => Promise<void>;
}

export default function UserPlaylist({playlist, goBack, guildId, onRefresh}: UserPlaylistProps) {
    const [loading, setLoading] = useState<boolean>(false);
    const [playlistTracks, setPlaylistTracks] = useState<TrackDto[] | null>(playlist?.tracks ?? []);
    const [playLoading, setPlayLoading] = useState<boolean>(false);
    const [addToQueueLoading, setAddToQueueLoading] = useState<boolean>(false);
    const [errorPlaying, setErrorPlaying] = useState<boolean>(false);

    const onEdit = async () => {
        setLoading(true)
        await onRefresh();
        setLoading(false);
    }

    const onDelete = async () => {
        setLoading(true);
        await RequestDeletePlaylist(guildId, playlist!.id);
        await onRefresh();
        goBack();
    }

    const onDeleteTrack = async (trackId: string) => {
        setPlaylistTracks(playlistTracks!.filter((track) => track.id !== trackId));
        await RequestDeleteTrack(guildId, playlist!.id, trackId);
        await onRefresh();
    }

    const handleReorder = async (sourceIndex: number, destinationIndex: number) => {
        const reorderedQueue = Array.from(playlistTracks!);
        const [movedItem] = reorderedQueue.splice(sourceIndex, 1);
        reorderedQueue.splice(destinationIndex, 0, movedItem);
        setPlaylistTracks(reorderedQueue);

        await RequestReorderPlaylist(guildId, playlist!.id, sourceIndex, destinationIndex);
        await onRefresh();
    }

    const handleDragEnd = async (result: any) => {
        const {destination, source} = result;

        // If there is no destination (dropped outside the list), do nothing
        if (!destination) return;

        // If the item is dropped in the same place, do nothing
        if (destination.index === source.index) return;

        await handleReorder(source.index, destination.index);
    };

    const handlePlay = async () => {
        setPlayLoading(true);
        const response = await RequestPlayPlaylist(guildId, playlist!.id, true);
        setPlayLoading(false);

        if (response !== "") {
            setErrorPlaying(true);
        }
    }

    const handleAddToQueue = async () => {
        setAddToQueueLoading(true);
        const response = await RequestPlayPlaylist(guildId, playlist!.id, false);
        setAddToQueueLoading(false);

        if (response !== "") {
            setErrorPlaying(true);
        }
    }

    const getTotalDuration = () => {
        let duration: number = 0;
        playlist!.tracks.forEach((track) => {
            duration += track.durationInSeconds;
        });

        return formatDuration(duration);
    }

    const refreshPlaylist = async () => {
        setLoading(true);
        await onRefresh();
        setLoading(false);
    }

    useEffect(() => {
        setPlaylistTracks(playlist?.tracks ?? []);
    }, [playlist]);

    return (
        <>
            {errorPlaying && <ErrorAlert title="No voice channel"
                                         error="Please connect to a voice channel the bot can access and try again."
                                         onDismiss={() => setErrorPlaying(false)}/>}
            <AlertDialog>
                <div className="w-full h-full mt-3 ml-3">
                    <div className="w-[calc(100%-15px)] h-full">
                        <div className="flex items-center">
                            <ArrowLeftIcon onClick={goBack} className="w-8 h-8 text-white cursor-pointer"/>
                            <RefreshCwIcon onClick={refreshPlaylist}
                                           className="text-black dark:text-white cursor-pointer ml-2"/>
                        </div>
                        {playlist === null || playlist === undefined || loading ?
                            <UserPlaylistSkeleton/>
                            :
                            <div className="ml-3 mt-3 w-full max-w-[calc(100%-20px)] h-full">
                                <div className="flex max-w-[calc(100%)]">
                                    <div className="max-w-[calc(100%-100px)]">
                                        <div className="flex items-center">
                                            <div className="text-2xl dark:text-white text-black truncate">
                                                {playlist.name}
                                            </div>
                                            <div className="ml-2">{playlist.isPublic ? <div/> :
                                                <LockClosedIcon className="w-6 h-6"/>}
                                            </div>
                                            <PlaylistDialog guildId={guildId} refresh={onEdit} createType={false}
                                                            playlist={playlist}
                                                            child={
                                                                <div className="ml-2">
                                                                    <PenIcon
                                                                        className="h-6 w-6 dark:text-white text-black cursor-pointer"/>
                                                                </div>
                                                            }>
                                            </PlaylistDialog>
                                            <AlertDialogTrigger asChild>
                                                <div className="ml-2">
                                                    <TrashIcon
                                                        className="h-6 w-6 dark:text-white text-black cursor-pointer"/>
                                                </div>
                                            </AlertDialogTrigger>
                                        </div>
                                        <div className="flex items-center">
                                            <div
                                                className="text-xl truncate">by {playlist.ownerUsername}</div>
                                            <Avatar className="h-6 w-6 ml-2">
                                                <AvatarImage src={playlist.ownerAvatarUrl}/>
                                                <AvatarFallback>{playlist.ownerUsername[0]}</AvatarFallback>
                                            </Avatar>
                                        </div>
                                        <div className="text-l truncate">
                                            Duration: {getTotalDuration()}
                                        </div>
                                        <div className="text-l truncate">
                                            Songs: {playlist.tracks.length}
                                        </div>
                                    </div>
                                    <div className="ml-auto mr-2 mt-6 flex items-center space-x-2">
                                        {playLoading ?
                                            <Loader2 className="animate-spin h-10 w-10 text-primary"/>
                                            :
                                            <PlayIcon className="text-black dark:text-white w-10 h-10 cursor-pointer"
                                                      onClick={handlePlay}/>
                                        }
                                        {addToQueueLoading ?
                                            <Loader2 className="animate-spin h-10 w-10 text-primary"/>
                                            :
                                            <ListPlusIcon
                                                className="w-10 h-10 cursor-pointer text-black dark:text-white"
                                                onClick={handleAddToQueue}/>
                                        }
                                    </div>
                                </div>
                                <DragDropContext onDragEnd={handleDragEnd}>
                                    <Droppable droppableId="songQueue">
                                        {(provided) => (
                                            <ScrollArea
                                                className="w-full my-3 h-[calc(100%-130px)] rounded-l-3xl"
                                                {...provided.droppableProps}
                                                ref={provided.innerRef}
                                            >
                                                <div className="space-y-3 my-4">
                                                    {playlistTracks!.map((track, index) => (
                                                        <Draggable key={`item-${index}`} draggableId={`item-${index}`}
                                                                   index={index}>
                                                            {(provided) => (
                                                                <div
                                                                    ref={provided.innerRef}
                                                                    {...provided.draggableProps}
                                                                    {...provided.dragHandleProps}
                                                                >
                                                                    <PlaylistTrack track={track} guildId={guildId}
                                                                                   onRemove={onDeleteTrack}
                                                                                   playlistId={playlist.id}/>
                                                                </div>
                                                            )}
                                                        </Draggable>
                                                    ))}
                                                    {provided.placeholder}
                                                </div>
                                            </ScrollArea>
                                        )}
                                    </Droppable>
                                </DragDropContext>
                            </div>
                        }
                    </div>
                </div>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                        <AlertDialogDescription>
                            This cannot be undone.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction onClick={onDelete}>Delete
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}