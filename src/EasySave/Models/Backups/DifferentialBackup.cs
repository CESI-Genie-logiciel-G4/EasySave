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
        
        var needEncryption = job.UseEncryption &&
                            ExtensionService.EncryptedExtensions.Contains(Path.GetExtension(sourceFile));
        
        if (!CryptoService.AreFilesIdentical(sourceFile, lastFullBackupFile!, needEncryption))
        {
            CryptoService.SecureCopy(sourceFile, destinationFile, job);
        }
    }
}