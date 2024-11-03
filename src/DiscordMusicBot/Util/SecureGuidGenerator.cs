using System.Security.Cryptography;

namespace DiscordMusicBot.Util;

public static class SecureGuidGenerator
{
    public static Guid CreateCryptographicallySecureGuid()
    {
        using var provider = RandomNumberGenerator.Create();
        var bytes = new byte[16];
        provider.GetBytes(bytes);

        return new Guid(bytes);
    }
}