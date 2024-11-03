namespace DiscordMusicBot.Services;

public static class ConfigService
{
    public static string DbConnectionString { get; } = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                                                       throw new ArgumentNullException(nameof(DbConnectionString));

    public static string BotToken { get; } = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ??
                                             throw new ArgumentNullException(nameof(BotToken));

    public static string LavaLinkConnectionString { get; } =
        Environment.GetEnvironmentVariable("LAVA_LINK_CONNECTION_STRING") ??
        throw new ArgumentNullException(nameof(LavaLinkConnectionString));

    public static string LavaLinkPassword { get; } = Environment.GetEnvironmentVariable("LAVA_LINK_PASSWORD") ??
                                                     throw new ArgumentNullException(nameof(LavaLinkPassword));

    public static ulong DebugGuildId { get; } =
        ulong.Parse(Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") ?? "0");

    public static string FrontendBaseUrl { get; } =
        Environment.GetEnvironmentVariable("FRONTEND_BASE_URL") ?? "http://localhost:3000";
}