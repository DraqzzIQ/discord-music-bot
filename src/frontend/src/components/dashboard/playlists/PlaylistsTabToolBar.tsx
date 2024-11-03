import SearchInput from "@/components/ui/search-input";
import DefaultButton from "@/components/DefaultButton";
import {PlusIcon, RefreshCwIcon} from "lucide-react";
import React from "react";
import {Checkbox} from "@/components/ui/checkbox";
import PlaylistDialog from "@/components/dashboard/playlists/PlaylistDialog";

interface PlaylistsTabToolBarProps {
    onCheckedChange: (checked: boolean) => void;
    onQueryChange: (query: string) => void;
    onRefresh: () => Promise<void>;
    guildId: number;
    checked: boolean;
    query: string;
}

export default function PlaylistsTabToolBar({
                                                onCheckedChange,
                                                onQueryChange,
                                                onRefresh,
                                                guildId,
                                                checked,
                                                query
                                            }: PlaylistsTabToolBarProps) {
    const [refreshing, setRefreshing] = React.useState<boolean>(false);

    const refresh = async () => {
        setRefreshing(true);
        await onRefresh();
        setRefreshing(false);
    }

    return (
        <div className="flex justify-between items-center w-full">
            <DefaultButton tooltipText="Refresh" className="w-8 h-8 ml-3 hover:scale-110 mr-2" onClick={refresh}>
                <RefreshCwIcon className={refreshing ? "animate-spin" : ""} size="26"/>
            </DefaultButton>
            <div className="flex justify-center items-center flex-grow">
                <SearchInput onSearch={() => {
                }} onChange={onQueryChange} placeholder="Search for a playlist" passedQuery={query}/>
                <div className="flex items-center space-x-2 ml-3">
                    <Checkbox className="w-5 h-5" onCheckedChange={onCheckedChange} defaultChecked={checked}/>
                    <label
                        className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                        Only mine
                    </label>
                </div>
            </div>
            <PlaylistDialog createType={true} guildId={guildId} refresh={refresh} child={
                <DefaultButton tooltipText="Create new Playlist" className="w-8 h-8 ml-3 hover:scale-110 mr-2">
                    <PlusIcon size="32"/>
                </DefaultButton>}
            />
        </div>

    )
}