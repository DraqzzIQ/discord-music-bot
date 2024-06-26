namespace DMusicBot;
public class Config
{
    public static readonly string BotToken = Environment.GetEnvironmentVariable("MUSIC_BOT_TOKEN") ?? throw new ArgumentNullException(nameof(BotToken));
    public static readonly string LavaLinkConnectionString = Environment.GetEnvironmentVariable("LAVA_LINK_CONNECTION_STRING") ?? throw new ArgumentNullException(nameof(LavaLinkConnectionString));
    public static readonly string LavaLinkPassword = Environment.GetEnvironmentVariable("LAVA_LINK_PASSWORD") ?? throw new ArgumentNullException(nameof(LavaLinkPassword));
    public static readonly string DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new ArgumentNullException(nameof(DbConnectionString));
    public static readonly ulong DebugGuildId = ulong.Parse(Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") ?? "0");
}
