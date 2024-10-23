"use client"

import * as React from "react"
import * as SliderPrimitive from "@radix-ui/react-slider"
import {Loader2} from "lucide-react"

import {cn} from "@/lib/utils"

interface SliderProps extends React.ComponentPropsWithoutRef<typeof SliderPrimitive.Root> {
    loading?: boolean
}

const Slider = React.forwardRef<
    React.ElementRef<typeof SliderPrimitive.Root>,
    SliderProps
>(({className, loading = false, ...props}, ref) => (
    <SliderPrimitive.Root
        ref={ref}
        className={cn(
            "relative flex w-full touch-none select-none items-center",
            className,
            {'opacity-50 pointer-events-none': props.disabled || loading}
        )}
        {...props}
    >
        <SliderPrimitive.Track className="relative h-1.5 w-full grow overflow-hidden rounded-full bg-primary/20">
            <SliderPrimitive.Range className="absolute h-full bg-primary"/>
        </SliderPrimitive.Track>
        {loading ? (
            <Loader2 className="animate-spin h-4 w-4 text-primary"/>
        ) : (
            <SliderPrimitive.Thumb
                className="block h-4 w-4 rounded-full border border-primary bg-background shadow transition-colors focus-visible:outline-none hover:cursor-pointer hover:scale-125"/>
        )}
    </SliderPrimitive.Root>
))
Slider.displayName = SliderPrimitive.Root.displayName

export {Slider}