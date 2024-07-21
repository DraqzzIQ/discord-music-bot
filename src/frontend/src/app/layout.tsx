import React from "react";
import "@/styles/globals.css";
import type {Metadata} from "next";
import {Inter as FontSans} from "next/font/google";
import {ThemeProvider} from "@/components/root/ThemeProvider";
import {ThemeToggle} from "@/components/root/ThemeToggle";

import {cn} from "@/lib/utils";

const fontSans = FontSans({
    subsets: ["latin"],
    variable: "--font-sans",
})

export const metadata: Metadata = {
    title: "Music Bot Manager",
};

export default function RootLayout({
                                       children,
                                   }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en" suppressHydrationWarning>
        <body className={cn("min-h-screen font-sans antialiased",
            fontSans.variable
        )}>
        <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
            <ThemeToggle/>
            <div className="bg-background text-foreground flex-1">
                {children}
            </div>
        </ThemeProvider>
        </body>
        </html>
    );
}