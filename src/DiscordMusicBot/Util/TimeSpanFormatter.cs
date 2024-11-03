namespace DiscordMusicBot.Util;

internal static class TimeSpanFormatter
{
    public static string FormatDuration(TimeSpan? timeSpan)
    {
        if (timeSpan is null)
            return "";

        string formatted;

        if (timeSpan.Value.Days > 0)
            formatted = timeSpan.Value.ToString(@"d\.hh\:mm\:ss");
        else if (timeSpan.Value.Hours > 0)
            formatted = timeSpan.Value.ToString(@"hh\:mm\:ss");
        else
            formatted = timeSpan.Value.ToString(@"mm\:ss");

        return formatted;
    }
}