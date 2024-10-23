'use client'

import { useState } from 'react'
import { Button } from "@/components/ui/button"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { InfoIcon } from "lucide-react"

interface InfoIconProps {
  buttonLabel: string
  popoverTitle: string
  popoverContent: string
}

function parseBoldText(text: string) {
  const parts = text.split(/(\*[^*]+\*)/)
  return parts.map((part, index) => {
    if (part.startsWith('*') && part.endsWith('*')) {
      return <strong key={index}>{part.slice(1, -1)}</strong>
    }
    return part
  })
}

export default function InfoTooltip({ 
  buttonLabel = "Show information", 
  popoverTitle = "Information", 
  popoverContent = "This is some helpful information."
}: InfoIconProps) {
  const [open, setOpen] = useState(false)

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button 
          variant="ghost" 
          size="icon"
          className="rounded-full"
          aria-label={buttonLabel}
        >
          <InfoIcon className="h-5 w-5" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-80">
        <div className="space-y-2">
          <h3 className="font-medium leading-none">{popoverTitle}</h3>
          <p className="text-sm text-muted-foreground">
            {parseBoldText(popoverContent)}
          </p>
        </div>
      </PopoverContent>
    </Popover>
  )
}