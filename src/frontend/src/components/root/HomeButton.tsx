"use client";

import DefaultButton from "@/components/DefaultButton";
import { useRouter } from "next/navigation";

export default function HomeButton() {
    const router = useRouter();
    return (
        <DefaultButton variant="outline" size="icon" tooltipText="Home" className="hover:scale-100 hover:bg-muted m-0.5"
            onClick={() => router.push("/")}>
            <img src="/home-icon.svg" alt="home" className="h-10 w-10 dark:invert"/>
        </DefaultButton>
    )
}