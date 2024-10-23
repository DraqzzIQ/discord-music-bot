import React, {useState} from "react";
import DefaultButton from "@/components/DefaultButton";
import {Loader2, MenuIcon, Shuffle, Trash2} from "lucide-react";
import {RequestClear, RequestDeduplicate, RequestShuffle} from "@/api/rest/apiService";


export interface QueueControlsProps {
    guildId: number;
}

const QueueControls: React.FC<QueueControlsProps> = ({guildId}) => {
    const [shuffleLoading, setShuffleLoading] = useState(false);
    const [clearLoading, setClearLoading] = useState(false);
    const [deduplicateLoading, setDeduplicateLoading] = useState(false);

    const handleShuffle = async () => {
        setShuffleLoading(true);
        await RequestShuffle(guildId);
        setShuffleLoading(false);
    }

    const handleClear = async () => {
        setClearLoading(true);
        await RequestClear(guildId);
        setClearLoading(false);
    }

    const handleDeduplicate = async () => {
        setDeduplicateLoading(true);
        await RequestDeduplicate(guildId);
        setDeduplicateLoading(false);
    }

    return (
        <div className="w-full mt-2 h-10">
            <div className="flex space-x-1 w-full justify-center">
                <DefaultButton tooltipText="Shuffle Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-32 mx-0 flex-1"
                               onClick={handleShuffle}
                               disabled={shuffleLoading}>
                    {shuffleLoading ?
                        <Loader2 className="w-5 h-5 mr-2 animate-spin text-primary"/>
                        :
                        <Shuffle className="w-5 h-5 mr-2"/>
                    }
                    Shuffle
                </DefaultButton>
                <DefaultButton tooltipText="Clear Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-28 mr-0 flex-1"
                               onClick={handleClear}
                               disabled={clearLoading}>
                    {clearLoading ?
                        <Loader2 className="w-5 h-5 mr-2 animate-spin text-primary"/>
                        :
                        <Trash2 className="w-5 h-5 mr-2"/>
                    }
                    Clear
                </DefaultButton>
                <DefaultButton tooltipText="Clear Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-40 mr-0 flex-1"
                               onClick={handleDeduplicate}
                               disabled={deduplicateLoading}>
                    {deduplicateLoading ?
                        <Loader2 className="w-5 h-5 mr-2 animate-spin text-primary"/>
                        :
                        <MenuIcon className="w-5 h-5 mr-2"/>
                    }
                    Deduplicate
                </DefaultButton>
            </div>
        </div>
    );
};

export default QueueControls;