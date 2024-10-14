import Playlists from "@/components/playlists/Playlists";
import PlaylistSkeleton from "@/components/skeletons/PlaylistSkeleton";

export default function PlaylistTab() {
    return(
        <Playlists>
            {Array.from({length: 10}, (_, index) => (
                <PlaylistSkeleton key={index}/>
            ))}
        </Playlists>
    )
}