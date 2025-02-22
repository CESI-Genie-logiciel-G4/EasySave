using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models.Backups;

public class FullBackup() : BackupType("FullBackup")
{
    public override void Initialize(BackupJob job)
    {
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        FileHelper.Copy(sourceFile, destinationFile, job);

        if (!job.UseEncryption) return;

        var encryptedExtensions = ExtensionService.EncryptedExtensions;
        if (!encryptedExtensions.Contains(Path.GetExtension(sourceFile))) return;

        CryptoService.EncryptFile(destinationFile);
    }
}
