namespace DMusicBot.Extensions;

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

        List<T> buffer = source.ToList();
        for (int i = 0; i < buffer.Count; i++)
        {
            int j = random.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}