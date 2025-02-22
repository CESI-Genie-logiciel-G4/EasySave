using System.Security.Cryptography;

namespace EasySave.Services;

public static class CryptoService
{
    public static void EncryptFile(string filePath)
    {
        var key = KeyManager.GetOrCreateKey();
        var iv = KeyManager.GetOrCreateIv();

        var encryptedFilePath = filePath + ".aes";

        try
        {
            using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            inputStream.CopyTo(cryptoStream);
            cryptoStream.FlushFinalBlock();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Encryption failed for {filePath}: {ex.Message}");
            return;
        }

        File.Delete(filePath);
    }
}