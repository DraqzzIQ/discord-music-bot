# DMusicBot

Music Bot for Discord written in C# using [Lavalink4NET](https://github.com/angelobreuer/Lavalink4NET) and [Discord.NET](https://github.com/discord-net/Discord.Net)


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
- Playlists

## Environment Variables
```
{
    export MUSIC_BOT_TOKEN=bot-token // Required, Discord Bot Token
    export LAVA_LINK_CONNECTION_STRING=http://127.0.0.1:2333 // Required, Lavalink Connection String
    export LAVA_LINK_PASSWORD=lavalink-password // Required, Lavalink Password
    export DB_CONNECTION_STRING=connection-string // Required, Database Connection String (MongoDB)
    export DEBUG_GUILD_ID=1234567890 // Optional, used for debugging purposes
}
```