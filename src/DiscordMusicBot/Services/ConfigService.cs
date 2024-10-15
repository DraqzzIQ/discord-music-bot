﻿namespace DiscordMusicBot.Services;
public class ConfigService
{
    public string DbConnectionString { get; } = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new ArgumentNullException(nameof(DbConnectionString));
    public string BotToken { get; } = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new ArgumentNullException(nameof(BotToken));
    public string LavaLinkConnectionString { get; } = Environment.GetEnvironmentVariable("LAVA_LINK_CONNECTION_STRING") ?? throw new ArgumentNullException(nameof(LavaLinkConnectionString));
    public string LavaLinkPassword { get; } = Environment.GetEnvironmentVariable("LAVA_LINK_PASSWORD") ?? throw new ArgumentNullException(nameof(LavaLinkPassword));
    public ulong DebugGuildId { get; } = ulong.Parse(Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") ?? "0");
    public string FrontendBaseUrl { get; } = Environment.GetEnvironmentVariable("FRONTEND_BASE_URL") ?? "http://localhost:3000";
}