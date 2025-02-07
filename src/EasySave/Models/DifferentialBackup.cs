using EasySave.Helpers;

namespace EasySave.Models;

public class DifferentialBackup() : BackupType("DifferentialBackup")
{
    public override void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job)
    {
        if (!File.Exists(destinationFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destinationFile))
        {
            FileHelper.Copy(sourceFile, destinationFile, job);
            Console.WriteLine($"Copied: {sourceFile} to {destinationFile}");
            execution.UpdateProgress(10);
        }
    }
}