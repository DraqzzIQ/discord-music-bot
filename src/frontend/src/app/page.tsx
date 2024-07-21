"use client";

import PlayerControl from "@/components/player/PlayerControl";
import Playlists from "@/components/playlists/Playlists";
import QueuedSongs from "@/components/queue/QueuedSongs";
import React from "react";

const Home: React.FC = () => {
    const [showQueue, setShowQueue] = React.useState(true);

    return (
        <div className="flex flex-col h-screen">
            <div className="flex flex-row flex-grow overflow-hidden">
                <Playlists/>
                {showQueue && <QueuedSongs/>}
            </div>
            <PlayerControl toggleQueue={() => setShowQueue(!showQueue)}/>
        </div>
    );
};

export default Home;