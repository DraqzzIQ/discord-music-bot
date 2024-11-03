"use client";

import React from "react";
import {ScrollArea} from "@/components/ui/scroll-area";
import QueueControls from "@/components/queue/QueueControls";
import {DragDropContext, Draggable, Droppable} from '@hello-pangea/dnd';
import {RequestReorder} from "@/api/rest/apiService";

interface QueuedSongsProps {
    children: React.ReactNode;
    guildId: number;
    onReorder: (sourceIndex: number, destinationIndex: number) => Promise<void>;
}

const SongQueue: React.FC<QueuedSongsProps> = ({children, guildId, onReorder}) => {
    const childrenArray = React.Children.toArray(children);
    const handleReorder = async (sourceIndex: number, destinationIndex: number) => {
        await RequestReorder(guildId, sourceIndex, destinationIndex);
    }

    const handleDragEnd = async (result: any) => {
        const {destination, source} = result;

        // If there is no destination (dropped outside the list), do nothing
        if (!destination) return;

        // If the item is dropped in the same place, do nothing
        if (destination.index === source.index) return;

        await onReorder(source.index, destination.index);
        await handleReorder(source.index, destination.index);
    };

    return (
        <div className="w-1/3 flex flex-col border-2 rounded-3xl mt-4 mb-2 mr-3">
            <QueueControls guildId={guildId}/>
            <div className="border-2 rounded-3xl m-2 mt-4 overflow-hidden flex h-full">
                <DragDropContext onDragEnd={handleDragEnd}>
                    <Droppable droppableId="songQueue">
                        {(provided) => (
                            <ScrollArea
                                className="w-full my-3 ml-2 mr-2 overflow-y-auto flex-grow, rounded-l-3xl"
                                {...provided.droppableProps}
                                ref={provided.innerRef}
                            >
                                <div className="space-y-3 h-full">
                                    {childrenArray.map((child, index) => (
                                        <Draggable key={`item-${index}`} draggableId={`item-${index}`} index={index}>
                                            {(provided) => (
                                                <div
                                                    ref={provided.innerRef}
                                                    {...provided.draggableProps}
                                                    {...provided.dragHandleProps}
                                                >
                                                    {child}
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
        </div>
    );
};

export default SongQueue;