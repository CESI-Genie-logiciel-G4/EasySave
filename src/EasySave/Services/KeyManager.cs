using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using EasySave.Helpers;

namespace EasySave.Services;

public sealed class KeyManager
{
    private static KeyManager? _instance;

    private const string KeyPath = ".easysave/security/encryption_key";
    private const string IvPath = ".easysave/security/encryption_iv";

    private readonly byte[] _key;
    private readonly byte[] _iv;

    private KeyManager()
    {
        _key = LoadOrCreateKey();
        _iv = LoadOrCreateIv();
    }

    [MethodImpl (MethodImplOptions.Synchronized)]
    public static KeyManager GetInstance()
    {
        return _instance ??= new KeyManager();
    }

    public byte[] GetKey() => _key;
    public byte[] GetIv() => _iv;

    private static byte[] LoadOrCreateKey()
    {
        if (File.Exists(KeyPath))
        {
            return Convert.FromBase64String(File.ReadAllText(KeyPath));
        }

        using var aes = Aes.Create();
        aes.GenerateKey();
        FileHelper.CreateAndWrite(KeyPath, Convert.ToBase64String(aes.Key));
        return aes.Key;
    }

    private static byte[] LoadOrCreateIv()
    {
        if (File.Exists(IvPath))
        {
            return Convert.FromBase64String(File.ReadAllText(IvPath));
        }

        using var aes = Aes.Create();
        aes.GenerateIV();
        FileHelper.CreateAndWrite(IvPath, Convert.ToBase64String(aes.IV));
        return aes.IV;
    }
}