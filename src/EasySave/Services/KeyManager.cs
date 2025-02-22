using System.Security.Cryptography;

namespace EasySave.Services;

public static class KeyManager
{
    private const string KeyPath = "encryption_key.txt";
    private const string IvPath = "encryption_iv.txt";

    public static byte[] GetOrCreateKey()
    {
        if (File.Exists(KeyPath))
        {
            return Convert.FromBase64String(File.ReadAllText(KeyPath));
        }

        using var aes = Aes.Create();
        aes.GenerateKey();
        File.WriteAllText(KeyPath, Convert.ToBase64String(aes.Key));
        return aes.Key;
    }

    public static byte[] GetOrCreateIv()
    {
        if (File.Exists(IvPath))
        {
            return Convert.FromBase64String(File.ReadAllText(IvPath));
        }

        using var aes = Aes.Create();
        aes.GenerateIV();
        File.WriteAllText(IvPath, Convert.ToBase64String(aes.IV));
        return aes.IV;
    }
}