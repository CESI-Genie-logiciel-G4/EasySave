
using EasySave.Helpers;

namespace EasySave.Models;

public class FullBackup() : BackupType("FullBackup")
{
    public override void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job)
    {
        FileHelper.Copy(sourceFile, destinationFile, job);
        execution.UpdateProgress(10);
    }
}
