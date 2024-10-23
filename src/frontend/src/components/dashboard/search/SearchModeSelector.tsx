import * as React from "react"
import { CaretSortIcon, CheckIcon } from "@radix-ui/react-icons"

import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
    Command,
    CommandEmpty,
    CommandGroup,
    CommandInput,
    CommandItem,
    CommandList,
} from "@/components/ui/command"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"
import {TrackSearchMode} from "@/datatypes/TrackSearchMode";

const searchMods = [
    {
        value: TrackSearchMode.Deezer.prefix,
        label: "Deezer",
    },
    {
        value: TrackSearchMode.Spotify.prefix,
        label: "Spotify",
    },
    {
        value: TrackSearchMode.YouTube.prefix,
        label: "YouTube",
    },
    {
        value: TrackSearchMode.YouTubeMusic.prefix,
        label: "YouTubeMusic",
    },
    {
        value: TrackSearchMode.Link.prefix,
        label: "Link",
    },
]

export interface SearchSelectorProps {
    onSelect: (value: string) => void
}

export function SearchModeSelector({ onSelect }: SearchSelectorProps) {
    const [open, setOpen] = React.useState(false)
    const [value, setValue] = React.useState(TrackSearchMode.Deezer.prefix)

    return (
        <Popover open={open} onOpenChange={setOpen}>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={open}
                    className="w-[200px] justify-between"
                >
                    {value
                        ? searchMods.find((searchMode) => searchMode.value === value)?.label
                        : "Select Source..."}
                    <CaretSortIcon className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
            </PopoverTrigger>
            <PopoverContent className="w-[200px] p-0">
                <Command>
                    <CommandInput placeholder="Search Sources..." className="h-9" />
                    <CommandList>
                        <CommandEmpty>No source found.</CommandEmpty>
                        <CommandGroup>
                            {searchMods.map((searchMode) => (
                                <CommandItem
                                    key={searchMode.value}
                                    value={searchMode.value}
                                    onSelect={(currentValue) => {
                                        const newValue = currentValue === value ? "" : currentValue
                                        setValue(newValue)
                                        onSelect(newValue)
                                        setOpen(false)
                                    }}
                                >
                                    {searchMode.label}
                                    <CheckIcon
                                        className={cn(
                                            "ml-auto h-4 w-4",
                                            value === searchMode.value ? "opacity-100" : "opacity-0"
                                        )}
                                    />
                                </CommandItem>
                            ))}
                        </CommandGroup>
                    </CommandList>
                </Command>
            </PopoverContent>
        </Popover>
    )
}