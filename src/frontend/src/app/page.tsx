import Player from "@/components/player/Player";
import Playlists from "@/components/playlists/Playlists";
import QueuedSongs from "@/components/queue/QueuedSongs";

const Home: React.FC = () => {
  return (
      <div className="flex flex-col h-screen">
        <div className="flex flex-grow overflow-hidden">
          <Playlists />
          <QueuedSongs />
        </div>
        <Player />
      </div>
  );
};

export default Home;