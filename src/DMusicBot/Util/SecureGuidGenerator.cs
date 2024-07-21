namespace DMusicBot.Util;

public static class SecureGuidGenerator
{
    public static Guid CreateCryptographicallySecureGuid()
    {
        using var provider = System.Security.Cryptography.RandomNumberGenerator.Create();
        byte[] bytes = new byte[16];
        provider.GetBytes(bytes);

        return new Guid(bytes);
    }
}