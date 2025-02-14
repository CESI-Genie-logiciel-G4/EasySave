
using EasySave.Helpers;

namespace EasySave.Models;

public class FullBackup() : BackupType("FullBackup")
{
    public override void Initialize(BackupJob job)
    {
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        FileHelper.Copy(sourceFile, destinationFile, job);
    }
}
