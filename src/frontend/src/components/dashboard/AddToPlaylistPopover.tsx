import {Popover, PopoverContent, PopoverTrigger} from '@/components/ui/popover';
import SearchInput from "@/components/ui/search-input";
import {Label} from "@/components/ui/label";
import React, {useState} from "react";
import {TrackDto} from "@/dtos/TrackDto";
import AddItemToPlaylistDto from "@/dtos/AddItemToPlaylistDto";
import {
    RequestAddPlaylist,
    RequestAddPlaylistToPlaylistPreviews,
    RequestAddTrack,
    RequestAddTrackToPlaylistPreviews,
    RequestDeleteTrack
} from "@/api/rest/apiService";
import {ScrollArea} from "@/components/ui/scroll-area";
import {Checkbox} from "@/components/ui/checkbox";
import {CheckedState} from "@radix-ui/react-checkbox";
import {PlaylistDto} from "@/dtos/PlaylistDto";

interface AddToPlaylistPopoverProps {
    child: React.ReactNode;
    track?: TrackDto;
    playlist?: PlaylistDto;
    guildId: number;
}

export default function AddToPlaylistPopover({child, playlist, track, guildId}: AddToPlaylistPopoverProps) {
    const [query, setQuery] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [addItemToPlaylistDtos, setAddItemToPlaylistDtos] = useState<AddItemToPlaylistDto[] | null>(null);
    const [filteredAddItemToPlaylistDtos, setFilteredAddItemToPlaylistDtos] = useState<AddItemToPlaylistDto[] | null>(null);

    const onQueryChange = (query: string) => {
        setQuery(query);
        filterAddItemToPlaylistDtos(query, addItemToPlaylistDtos);
    }

    const filterAddItemToPlaylistDtos = (query: string, addItemToPlaylistDtos: AddItemToPlaylistDto[] | null) => {
        if (!addItemToPlaylistDtos) {
            return;
        }
        if (!query) {
            setFilteredAddItemToPlaylistDtos(addItemToPlaylistDtos);
            return;
        }
        setFilteredAddItemToPlaylistDtos(addItemToPlaylistDtos.filter(p => p.name.toLowerCase().includes(query.trim().toLowerCase())));
    }

    const onOpenChange = async (open: boolean) => {
        if (!open) return;

        setLoading(true);
        let response: AddItemToPlaylistDto[] | null = null;
        if (playlist) {
            response = await RequestAddPlaylistToPlaylistPreviews(guildId, playlist.url, playlist.encodedTracks);
        } else if (track) {
            response = await RequestAddTrackToPlaylistPreviews(guildId, track.url!);
        }

        setAddItemToPlaylistDtos(response);
        filterAddItemToPlaylistDtos(query, response);
        setLoading(false);
    }

    const onCheckedChange = async (checked: CheckedState, playlistId: string) => {
        if (checked) {
            if (playlist) {
                await RequestAddPlaylist(guildId, playlistId, playlist.url, playlist.encodedTracks);
            } else if (track) {
                await RequestAddTrack(guildId, playlistId, track);
            }
        } else {
            if (track) {
                await RequestDeleteTrack(guildId, playlistId, undefined, track.url);
            } else {
            }
        }
    }

    return (
        <Popover onOpenChange={onOpenChange}>
            <PopoverTrigger asChild>
                {child}
            </PopoverTrigger>
            <PopoverContent className="w-96">
                <div className="grid gap-4">
                    <h4 className="font-medium leading-none">Add to playlist</h4>
                    <SearchInput onSearch={() => {
                    }} placeholder="Search for a playlist" onChange={onQueryChange}/>
                    <ScrollArea className="w-full max-h-[200px]">
                        {loading ? <div>Loading...</div> : filteredAddItemToPlaylistDtos?.map((playlist) => (
                            <div key={playlist.id} className="flex items-center mt-6">
                                <div
                                    className={`h-[52px] w-[52px] rounded-xl grid ${playlist.previewUrls.length > 0 ? 'grid-cols-2' : ''} overflow-hidden`}>
                                    {playlist.previewUrls.length === 0 &&
                                        <img src="/bluray-disc-icon.svg" alt={playlist.name}
                                             className="w-[52px] h-[52px] p-[5px] dark:invert"/>}
                                    <img src={playlist.previewUrls[0] ?? "/transparent-square.svg"}
                                         alt={playlist.name}
                                         className="w-[26px] h-[26px] object-cover"/>
                                    <img src={playlist.previewUrls[1] ?? "/transparent-square.svg"}
                                         alt={playlist.name}
                                         className="w-[26px] h-[26px] object-cover"/>
                                    <img src={playlist.previewUrls[2] ?? "/transparent-square.svg"}
                                         alt={playlist.name}
                                         className="w-[26px] h-[26px] object-cover"/>
                                    <img src={playlist.previewUrls[3] ?? "/transparent-square.svg"}
                                         alt={playlist.name}
                                         className="w-[26px] h-[26px] object-cover"/>
                                </div>
                                <Label className="ml-4 w-60 truncate text-xl">{playlist.name}</Label>
                                <Checkbox className="ml-auto mr-5 h-6 w-6"
                                          onCheckedChange={(e) => onCheckedChange(e, playlist.id)}
                                          defaultChecked={playlist.containsItem}
                                          disabled={playlist.containsItem && !track}/>
                            </div>
                        ))}
                    </ScrollArea>
                </div>
            </PopoverContent>
        </Popover>
    );
}