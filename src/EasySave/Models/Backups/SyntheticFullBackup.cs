using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models.Backups;

public class SyntheticFullBackup() : BackupType("SyntheticFullBackup")
{
    public override bool NeedToClearFolder => false;
    public override void Initialize(BackupJob job)
    {
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        if (!File.Exists(destinationFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destinationFile))
        {
            CryptoService.SecureCopy(sourceFile, destinationFile, job);
        }
    }
}
