# DMusicBot

Simple Music Bot for Discord written in C# using [Lavalink4NET](https://github.com/angelobreuer/Lavalink4NET) and [Discord.NET](https://github.com/discord-net/Discord.Net)


## Features
- Pause / Resume
- Skip
- Stop
- List Queue
- Volume
- Loop
- Shuffle
- Lyrics
- Seek

## config.json
```
{
    "BotToken": "bot_token", // Discord bot token
    "LL_Hostname": "localhost", // Lavalink hostname
    "LL_Port": 2333, // Lavalink port
    "LL_Password": "youshallnotpass", // Lavalink password
    "DebugGuildId": 0, // Guild ID to register commands to for debugging
}
```