using EasySave.Helpers;

namespace EasySave.Models.Backups;

public class DifferentialBackup() : BackupType("DifferentialBackup")
{
    private string? _lastFullBackupFolder = null;
    
    public override void Initialize(BackupJob job)
    {
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        InitializeLastFullBackup(job);
        var lastFullBackupFile = FileHelper.GetMirrorFilePath(
            job.SourceFolder, 
            sourceFile, 
            _lastFullBackupFolder!
            );
        
        if (!File.Exists(lastFullBackupFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(lastFullBackupFile))
        {
            FileHelper.Copy(sourceFile, destinationFile, job);
        }
    }
    
    private void InitializeLastFullBackup(BackupJob job)
    {
        _lastFullBackupFolder ??= FolderHelper.GetLastCompleteBackupFolder(job.SourceFolder);
    }
}