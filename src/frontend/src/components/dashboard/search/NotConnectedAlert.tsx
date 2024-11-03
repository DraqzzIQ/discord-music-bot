import {Cross1Icon, ExclamationTriangleIcon} from "@radix-ui/react-icons";
import {Alert, AlertDescription, AlertTitle} from "@/components/ui/alert";
import React, {useEffect, useState} from "react";
import {Progress} from "@/components/ui/progress";

export function NotConnectedAlert() {
    const [visible, setVisible] = useState(true);
    const [progress, setProgress] = useState(100);

    useEffect(() => {
        const timer = setTimeout(() => {
            setVisible(false);
        }, 10000);

        const interval = setInterval(() => {
            setProgress((prev) => Math.max(prev - 0.5, 0));
        }, 50);

        return () => {
            clearTimeout(timer);
            clearInterval(interval);
        };
    }, []);

    if (!visible) return null;

    return (
        <div className="fixed bottom-4 left-4 w-96">
            <Alert variant="destructive" className="bg-black">
                <div className="flex justify-between items-center">
                    <div className="flex items-center">
                        <ExclamationTriangleIcon className="h-4 w-4"/>
                        <div className="ml-2">
                            <AlertTitle>No voice channel</AlertTitle>
                            <AlertDescription>Please connect to a voice channel the bot can access and try again.</AlertDescription>
                        </div>
                    </div>
                    <button onClick={() => setVisible(false)}
                            className="border rounded border-transparent hover:border-destructive p-1">
                        <Cross1Icon className="h-4 w-4"/>
                    </button>
                </div>
                <div className="-mb-2 mt-1">
                    <Progress value={progress} className="bg-red-950 [&>*]:bg-destructive"/>
                </div>
            </Alert>
        </div>
    );
}