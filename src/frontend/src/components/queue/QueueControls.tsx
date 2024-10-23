import React from "react";
import DefaultButton from "@/components/DefaultButton";
import {MenuIcon, Shuffle, Trash2} from "lucide-react";
import {RequestClear, RequestDeduplicate, RequestShuffle} from "@/api/rest/apiService";


export interface QueueControlsProps {
    guildId: number;
}

const QueueControls: React.FC<QueueControlsProps> = ({guildId}) => {

    const handleShuffle = async () => {
        await RequestShuffle(guildId);
    }

    const handleClear = async () => {
        await RequestClear(guildId);
    }

    const handleDeduplicate = async () => {
        await RequestDeduplicate(guildId);
    }

    return (
        <div className="w-full mt-2 h-10">
            <div className="flex space-x-1 w-full justify-center">
                <DefaultButton tooltipText="Shuffle Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-32 mx-0 flex-1"
                               onClick={handleShuffle}>
                    <Shuffle className="w-5 h-5 mr-2"/>
                    Shuffle
                </DefaultButton>
                <DefaultButton tooltipText="Clear Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-28 mr-0 flex-1"
                               onClick={handleClear}>
                    <Trash2 className="w-5 h-5 mr-2"/>
                    Clear
                </DefaultButton>
                <DefaultButton tooltipText="Clear Queue"
                               className="text-lg hover:scale-105 border-2 h-9 max-w-40 mr-0 flex-1"
                               onClick={handleDeduplicate}>
                    <MenuIcon className="w-5 h-5 mr-2"/>
                    Deduplicate
                </DefaultButton>
            </div>
        </div>
    );
};

export default QueueControls;