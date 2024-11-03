namespace DiscordMusicBot.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(Random.Shared);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(random);

        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = random.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}