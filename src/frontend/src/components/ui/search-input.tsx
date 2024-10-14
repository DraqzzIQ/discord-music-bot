import { useState } from 'react'
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Search } from "lucide-react"

export interface SearchInputProps {
    onSearch: (query: string) => void,
    onChange: (query: string) => void,
    placeholder?: string,    
}

export default function Component({ onSearch, onChange, placeholder = "Search..."}: SearchInputProps) {
  const [query, setQuery] = useState('')

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    onSearch(query)
  }

  return (
    <form onSubmit={handleSearch} className="flex w-full max-w-sm items-center space-x-2">
      <div className="relative flex-grow">
        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-gray-500" />
        <Input
          type="search"
          placeholder={placeholder}
          value={query}
          onChange={(e) => {setQuery(e.target.value); onChange(e.target.value)}}
          className="pl-8"
        />
      </div>
    </form>
  )
}