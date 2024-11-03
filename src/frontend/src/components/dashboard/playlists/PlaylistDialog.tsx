import {Button} from "@/components/ui/button"
import {Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger,} from "@/components/ui/dialog"
import {Input} from "@/components/ui/input"
import {Label} from "@/components/ui/label"
import React, {ChangeEvent} from "react";
import {Switch} from "@/components/ui/switch";
import InfoTooltip from "@/components/ui/info-tooltip";
import {ExclamationTriangleIcon, ReloadIcon} from "@radix-ui/react-icons";
import {Alert, AlertDescription, AlertTitle} from "@/components/ui/alert";
import {RequestCreatePlaylist, RequestEditPlaylist} from "@/api/rest/apiService";
import UserPlaylistPreviewDto from "@/dtos/UserPlaylistPreviewDto";
import UserPlaylistDto from "@/dtos/UserPlaylistDto";

interface PlaylistDialogProps {
    child?: React.ReactNode;
    guildId: number;
    refresh: () => Promise<void>;
    createType: boolean;
    playlist?: UserPlaylistPreviewDto | UserPlaylistDto;
    onClose?: () => void;
}

export default function PlaylistDialog({child, guildId, refresh, createType, playlist, onClose}: PlaylistDialogProps) {
    const [publicPlaylist, setPublicPlaylist] = React.useState<boolean>(playlist ? playlist.isPublic : false);
    const [name, setName] = React.useState<string>(playlist ? playlist.name : '');
    const [buttonDisabled, setButtonDisabled] = React.useState<boolean>(true);
    const [loading, setLoading] = React.useState<boolean>(false);
    const [error, setError] = React.useState<string | null>(null);
    const [isOpen, setIsOpen] = React.useState<boolean>(false);

    const onSubmit = async () => {
        setLoading(true);
        let response: any;
        if (createType) {
            response = await RequestCreatePlaylist(guildId, name.trim(), publicPlaylist);
        } else {
            response = await RequestEditPlaylist(guildId, playlist?.id ?? "", name.trim(), publicPlaylist);
        }
        if (response !== '') {
            setError(response.replace(/"/g, ''));
            setLoading(false);
        } else {
            onOpenChange(false);
            setIsOpen(false);
            await refresh();
        }
    }

    const onPublicChange = (isPublic: boolean) => {
        setPublicPlaylist(isPublic);
        const nameLengthError: boolean = name.trim().length === 0 || name.trim().length > 100;
        setButtonDisabled(nameLengthError || loading);
    }

    const onNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setName(e.target.value);
        const nameLengthError: boolean = e.target.value.trim().length === 0 || e.target.value.trim().length > 100;
        setButtonDisabled(nameLengthError || loading);
    }

    const onOpenChange = (isOpen: boolean) => {
        setIsOpen(isOpen);
        if (isOpen) {
            return;
        }
        setName(playlist ? playlist.name : '');
        setPublicPlaylist(playlist ? playlist.isPublic : false);
        setButtonDisabled(true);
        setLoading(false);
        setError(null);
        if (onClose)
            onClose();
    }

    return (
        <Dialog onOpenChange={onOpenChange} open={isOpen}>
            <DialogTrigger asChild>
                {child}
            </DialogTrigger>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>{createType ? "Create new Playlist" : "Edit Playlist"}</DialogTitle>
                </DialogHeader>
                <div className="grid gap-4 py-4">
                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label className="text-right">
                            Name
                        </Label>
                        <Input className="col-span-3" placeholder="Add a name" onChange={onNameChange} value={name}
                               disabled={loading}/>
                    </div>
                    <div className="grid grid-cols-4 items-center gap-4">
                        <Label className="text-right">Public</Label>
                        <div className="col-span-3 flex items-center">
                            <Switch onCheckedChange={onPublicChange} disabled={loading}
                                    defaultChecked={playlist?.isPublic}/>
                            <div className="ml-auto">
                                <InfoTooltip buttonLabel="Show help" popoverTitle="Set public"
                                             popoverContent="Public playlists can be edited by everyone."/>
                            </div>
                        </div>
                    </div>
                </div>
                {error &&
                    <Alert variant="destructive">
                        <ExclamationTriangleIcon className="h-4 w-4"/>
                        <AlertTitle>Error</AlertTitle>
                        <AlertDescription>
                            {error}
                        </AlertDescription>
                    </Alert>
                }
                <DialogFooter>
                    <Button disabled={loading || buttonDisabled} onClick={onSubmit}>
                        {loading ?
                            <ReloadIcon className="animate-spin w-5 h-5"/> : (createType ? "Create" : "Save Changes")}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
