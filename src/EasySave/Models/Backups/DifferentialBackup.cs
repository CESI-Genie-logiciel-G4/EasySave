using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models.Backups;

public class DifferentialBackup() : BackupType("DifferentialBackup")
{
    private string? _lastFullBackupFolder = null;
    
    public override void Initialize(BackupJob job)
    {
        _lastFullBackupFolder = HistoryService.GetLastCompleteBackupFolder(job.SourceFolder);
    }

    public override void Execute(string sourceFile, string destinationFile, BackupJob job)
    {
        var lastFullBackupFile = FileHelper.GetMirrorFilePath(
            job.SourceFolder, 
            sourceFile, 
            _lastFullBackupFolder!
            );
        
        if (!File.Exists(lastFullBackupFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(lastFullBackupFile))
        {
            FileHelper.Copy(sourceFile, destinationFile, job);
            
            if (!job.UseEncryption) return;

            var encryptedExtensions = ExtensionService.EncryptedExtensions;
            if (!encryptedExtensions.Contains(Path.GetExtension(sourceFile))) return;

            CryptoService.EncryptFile(destinationFile);
        }
    }
}