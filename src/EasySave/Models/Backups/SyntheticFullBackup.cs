using EasySave.Helpers;

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
        }
    }
}
