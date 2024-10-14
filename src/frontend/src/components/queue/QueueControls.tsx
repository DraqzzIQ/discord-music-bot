import React from "react";
import DefaultButton from "@/components/DefaultButton";
import {Shuffle, Trash2} from "lucide-react";
import {RequestClear, RequestShuffle} from "@/api/rest/apiService";


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

    return (
        <div className="w-full mt-2 flex justify-center h-10">
            <div className="flex space-x-10">
                <DefaultButton tooltipText="Shuffle Queue"
                               className="text-lg hover:scale-110 border-2 h-9 w-32"
                               onClick={handleShuffle}>
                    <Shuffle className="w-5 h-5 mr-2"/>
                    Shuffle
                </DefaultButton>
                <DefaultButton tooltipText="Clear Queue"
                               className="text-lg hover:scale-110 border-2 h-9 w-32"
                               onClick={handleClear}>
                    <Trash2 className="w-5 h-5 mr-2"/>
                    Clear
                </DefaultButton>
            </div>
        </div>
    );
};

export default QueueControls;