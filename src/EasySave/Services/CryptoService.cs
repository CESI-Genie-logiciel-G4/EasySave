using System.Diagnostics;
using System.Security.Cryptography;
using EasySave.Helpers;
using EasySave.Models;
using Logger;
using Logger.LogEntries;

namespace EasySave.Services;

public static class CryptoService
{
    
    /// <summary>
    ///  Encrypt a file from source to destination
    /// </summary>
    /// <param name="source"> The file to encrypt </param>
    /// <param name="destination"> The destination file </param>
    public static void EncryptFile(string source, string destination)
    {
        ProcessFile(source, destination, true);
    }

    /// <summary>
    ///  Decrypt a file from source to destination
    /// </summary>
    /// <param name="source"> The file to decrypt </param>
    /// <param name="destination"> The destination file </param>
    public static void DecryptFile(string source, string destination)
    {
        ProcessFile(source, destination, false);
    }

    /// <summary>
    ///  Encrypt or decrypt a file from source to destination
    /// </summary>
    /// <param name="source"> The file to process </param>
    /// <param name="destination"> The destination file </param>
    /// <param name="encrypt"> True to encrypt, false to decrypt </param>
    private static void ProcessFile(string source, string destination, bool encrypt)
    {
        var keyManager = KeyManager.GetInstance();

        var key = keyManager.GetKey();
        var iv = keyManager.GetIv();

        FileHelper.CreateFile(destination);

        using var inputStream = new FileStream(source, FileMode.Open, FileAccess.Read);
        using var outputStream = new FileStream(destination, FileMode.Create, FileAccess.Write);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var cryptoStream = new CryptoStream(
            encrypt ? outputStream : inputStream,
            encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor(),
            encrypt ? CryptoStreamMode.Write : CryptoStreamMode.Read
        );

        if (encrypt)
        {
            inputStream.CopyTo(cryptoStream);
            cryptoStream.FlushFinalBlock();
        }
        else
        {
            cryptoStream.CopyTo(outputStream);
        }
    }

    /// <summary>
    ///   Copy a file from source to destination, encrypting it if needed
    ///   Logs the operation
    /// </summary>
    /// <param name="sourceFile"> The file to copy </param>
    /// <param name="destinationFile"> The destination file </param>
    /// <param name="job"> The job that triggered the copy </param>
    public static void SecureCopy(string sourceFile, string destinationFile, BackupJob job)
    {
        var logger = Logger.Logger.GetInstance();
        Stopwatch copyWatch = new ();
        Stopwatch encryptWatch = new ();
        
        try 
        {
            var needEncryption = job.UseEncryption && 
                                 ExtensionService.EncryptedExtensions.Contains(Path.GetExtension(sourceFile));
        
            copyWatch.Start();
            
            if (needEncryption)
            {
                encryptWatch.Start();
                EncryptFile(sourceFile, destinationFile + ".aes");
                while (encryptWatch.ElapsedMilliseconds == 0); // Security to avoid seems null encryption
                encryptWatch.Stop();
            }
            else
            {
                FileHelper.Copy(sourceFile, destinationFile);
            }
            
            copyWatch.Stop();
            
        } catch (Exception e)
        {
            logger.Log(new GlobalLogEntry("Backup", "An error occurred during the backup job", new()
            {
                ["Type"] = "error",
                ["sourceFile"] = sourceFile,
                ["destinationFile"] = destinationFile,
                ["JobName"] = job.Name,
                ["Error"] = e.Message
            }));

            throw;
        }
        
        logger.Log(new CopyFileLogEntry(
            "Backup",
            sourceFile,
            destinationFile,
            new FileInfo(sourceFile).Length,
            copyWatch.ElapsedMilliseconds,
            encryptWatch.ElapsedMilliseconds
        ));
    }
    
    public static bool AreFilesIdentical(string sourceFile, string lastFullBackupFile, bool needDecryption)
    {
        var destinationFile = needDecryption ? lastFullBackupFile + ".aes" : lastFullBackupFile;
        
        if (!File.Exists(destinationFile))
        {
            return false;
        }

        return needDecryption ? AreEncryptedFilesIdentical(sourceFile, destinationFile) : AreNonEncryptedFilesIdentical(sourceFile, lastFullBackupFile);
    }

    private static bool AreNonEncryptedFilesIdentical(string sourceFile, string lastFullBackupFile)
    {
        var sourceFileInfo = new FileInfo(sourceFile);
        var lastFullBackupFileInfo = new FileInfo(lastFullBackupFile);

        return sourceFileInfo.Length == lastFullBackupFileInfo.Length &&
               sourceFileInfo.LastWriteTime <= lastFullBackupFileInfo.LastWriteTime;
    }

    private static bool AreEncryptedFilesIdentical(string sourceFile, string destinationFile)
    {
        using var lastFullBackupFileStream = DecryptFileToStream(destinationFile);
        using var sourceFileStream = new BufferedStream(new FileStream(sourceFile, FileMode.Open, FileAccess.Read));
        
        return AreFileStreamsIdentical(sourceFileStream, lastFullBackupFileStream);
    }

    private static bool AreFileStreamsIdentical(BufferedStream sourceFileStream, Stream lastFullBackupFileStream)
    {
        int sourceByte, backupByte;
        do
        {
            sourceByte = sourceFileStream.ReadByte();
            backupByte = lastFullBackupFileStream.ReadByte();
        } while (sourceByte == backupByte && sourceByte != -1);

        return sourceByte == backupByte;
    }

    private static CryptoStream DecryptFileToStream(string lastFullBackupFile)
    {
        var keyManager = KeyManager.GetInstance();
        var key = keyManager.GetKey();
        var iv = keyManager.GetIv();

        var inputStream = new BufferedStream(new FileStream(lastFullBackupFile, FileMode.Open, FileAccess.Read));
        var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var cryptoStream = new CryptoStream(
            inputStream,
            aes.CreateDecryptor(),
            CryptoStreamMode.Read
        );

        return cryptoStream;
    }
}