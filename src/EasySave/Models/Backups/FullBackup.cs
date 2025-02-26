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
        CryptoService.SecureCopy(sourceFile, destinationFile, job);
    }
}
