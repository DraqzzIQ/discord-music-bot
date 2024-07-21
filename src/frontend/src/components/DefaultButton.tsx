import React from "react";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

interface DefaultButtonProps {
    children: React.ReactNode;
    tooltipText: string;
    [x: string]: any;
    onClick?: () => void;
    className?: string;
    disabled?: boolean;
}

const DefaultButton: React.FC<DefaultButtonProps> = ({ children, tooltipText, onClick, className, disabled, ...props }) => (
    <TooltipProvider>
        <Tooltip>
            <TooltipTrigger asChild>
                <Button
                    className={`bg-transparent text-foreground p-0 m-1 hover:scale-150 hover:bg-transparent transition duration-200 shadow-none ${className}`}
                    onClick={onClick}
                    disabled={disabled}
                    {...props}
                >
                    {children}
                </Button>
            </TooltipTrigger>
            <TooltipContent>{tooltipText}</TooltipContent>
        </Tooltip>
    </TooltipProvider>
);

export default DefaultButton;