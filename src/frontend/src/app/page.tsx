"use client";

import {ScrollArea} from "@/components/ui/scroll-area";
import {LoginSuccessAlert} from "@/components/root/LoginSuccessAlert";
import {LoginFailedAlert} from "@/components/root/LoginFailedAlert";
import {RequestGuilds} from "@/api/rest/apiService";
import React, {useEffect, useState} from "react";
import {GuildDto} from "@/dtos/GuildDto";
import {DiscordLogo} from "@phosphor-icons/react";
import Loading from "@/app/loading";
import Link from "next/link";

function Home() {
    const [alertType, setAlertType] = useState<string | null>(null);
    const [guilds, setGuilds] = useState<GuildDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function fetchGuilds() {
            setLoading(true);
            try {
                const fetchedGuilds = await RequestGuilds();
                setGuilds(fetchedGuilds);
                setAlertType("success");
            } catch (error) {
                console.error("Failed to fetch guilds:", error);
                setAlertType("failed");
            } finally {
                setLoading(false);
            }
        }

        fetchGuilds();
    }, []);

    if (loading) {
        return <Loading/>;
    }

    return (
        <div>
            {alertType === "success" && <LoginSuccessAlert/>}
            {alertType === "failed" && <LoginFailedAlert/>}
            <div className="text-center my-4">
                <h1 className="text-3xl font-semibold">Select Server</h1>
            </div>
            <ScrollArea className="p-4 overflow-y-auto flex-grow">
                <div className="grid-responsive justify-center justify-items-center">
                    {guilds.length === 0 ? (
                        <p className="text-center text-gray-500">
                            No servers connected. Please
                            use <strong>/web-player</strong> command
                            in your Discord server to login.
                        </p>
                    ) : (
                        guilds.map((guild) => (
                            <Link key={guild.id} href={`/dash/${guild.id}`}>
                                <div className="flex flex-col items-center justify-center w-[150px]">
                                    {guild.iconUrl ? (
                                            <img src={guild.iconUrl} alt={`${guild.name} icon`}
                                                 className="rounded-full w-[150px]"/>)
                                        : (<DiscordLogo className="rounded-full" size={150}/>)}
                                    <p className="mt-2 font-semibold">{guild.name}</p>
                                </div>
                            </Link>
                        ))
                    )}
                </div>
            </ScrollArea>
        </div>
    );
}

export default Home;