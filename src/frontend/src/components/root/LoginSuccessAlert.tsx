import {Cross1Icon, CheckIcon} from "@radix-ui/react-icons";
import {Alert, AlertDescription, AlertTitle} from "@/components/ui/alert";
import {Progress} from "@/components/ui/progress";
import React, {useEffect, useState} from "react";

export function LoginSuccessAlert() {
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
            <Alert variant="success">
                <div className="flex justify-between items-center">
                    <div className="flex items-center">
                        <CheckIcon className="h-6 w-6"/>
                        <div className="ml-2">
                            <AlertTitle>Login Success</AlertTitle>
                            <AlertDescription>Successfully logged in.</AlertDescription>
                        </div>
                    </div>
                    <button onClick={() => setVisible(false)}
                            className="border rounded border-transparent hover:border-success p-1">
                        <Cross1Icon className="h-4 w-4"/>
                    </button>
                </div>
                <div className="-mb-2 mt-1">
                    <Progress value={progress} className="bg-green-950 [&>*]:bg-success"/>
                </div>
            </Alert>
        </div>
    );
}