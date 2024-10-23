import React from "react";
import "@/styles/globals.css";
import type {Metadata} from "next";
import {Inter as FontSans} from "next/font/google";
import {ThemeProvider} from "@/components/root/ThemeProvider";
import {ThemeToggle} from "@/components/root/ThemeToggle";

import {cn} from "@/lib/utils";
import HomeButton from "@/components/root/HomeButton";

const fontSans = FontSans({
    subsets: ["latin"],
    variable: "--font-sans",
})

export const metadata: Metadata = {
    title: "Music Bot Dashboard",
};

export default function RootLayout({
                                       children,
                                   }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en" suppressHydrationWarning>
        <body className={cn("h-full font-sans antialiased select-none",
            fontSans.variable
        )}>
        <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
            <div className="absolute top-4 left-4 z-50 flex flex-row">
                <HomeButton/>
                <ThemeToggle/>
            </div>
            <div className="bg-background text-foreground flex-1">
                {children}
            </div>
        </ThemeProvider>
        </body>
        </html>
    );
}
