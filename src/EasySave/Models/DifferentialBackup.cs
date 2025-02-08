using EasySave.Helpers;

namespace EasySave.Models;

public class DifferentialBackup() : BackupType("DifferentialBackup")
{
    private string? _lastFullBackupFile = null;
    public override void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job)
    {
        _lastFullBackupFile ??= FileHelper.GetMirrorFilePath(
            job.SourceFolder, 
            sourceFile, 
            FolderHelper.GetLastCompleteBackupFolder(job.SourceFolder)
            );
        
        if (!File.Exists(_lastFullBackupFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(_lastFullBackupFile))
        {
            FileHelper.Copy(sourceFile, destinationFile, job);
        }
        execution.UpdateProgress(1);
    }
}