using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models.Backups;

public class SyntheticFullBackup() : BackupType("SyntheticFullBackup")
{
    public override void Initialize(BackupJob job)
    {
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        if (!File.Exists(destinationFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destinationFile))
        {
            FileHelper.Copy(sourceFile, destinationFile, job);
            
            if (!job.UseEncryption) return;

            var encryptedExtensions = ExtensionService.EncryptedExtensions;
            if (!encryptedExtensions.Contains(Path.GetExtension(sourceFile))) return;

            CryptoService.EncryptFile(destinationFile);
        }
    }
}
