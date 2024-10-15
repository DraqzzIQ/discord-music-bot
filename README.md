# Discord Music Bot

Discord music bot with web interface written in C# using [Lavalink4NET](https://github.com/angelobreuer/Lavalink4NET), [Discord.NET](https://github.com/discord-net/Discord.Net) and Next.js.

## Features
- Play
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
- Web Interface

# Usage

## Docker

### Environment Variables

Define the following .env files in the root directory.

bot.env
```
{
    DISCORD_BOT_TOKEN=bot-token // Required, Discord Bot Token
    LAVA_LINK_CONNECTION_STRING=http://lavalink:2333 // Required, Lavalink Connection String
    LAVA_LINK_PASSWORD=lavalink-password // Required, Lavalink Password
    DB_CONNECTION_STRING=connection-string // Required, Database Connection String (MongoDB)
    DEBUG_GUILD_ID=1234567890 // Optional, used for debugging purposes
}
```

mongo.env
```
{
    MONGO_INITDB_ROOT_USERNAME=admin // Required, MongoDB Root Username
    MONGO_INITDB_ROOT_PASSWORD=password // Required, MongoDB Root Password
}
```

Create folders for MongoDB and Lavalink data in the root directory with correct permissions.
- `db-data`
- `lavalink`
- `lavalink/plugins`

Define `lavalink/application.yml`

example `application.yml` can be found [here](example.application.yml)

Lavalink needs to be configured with plugins:
- [youtube-source](https://github.com/lavalink-devs/youtube-source)
- [LavaSearch](https://github.com/topi314/LavaSearch)
- [LavaSrc](https://github.com/topi314/LavaSrc)
- [java-timed-lyrics](https://github.com/DuncteBot/java-timed-lyrics)


`docker compose up`