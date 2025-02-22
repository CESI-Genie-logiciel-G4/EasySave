using EasySave.Models.Backups;

namespace EasySave.Models;

public class BackupJob(string name, string sourceFolder, string destinationFolder, BackupType backupType, bool useEncryption)
{
    public string Name { get; } = name;
    public string SourceFolder { get; } = sourceFolder;
    public string DestinationFolder { get; } = destinationFolder;
    public BackupType BackupType { get; } = backupType;
    public bool UseEncryption { get; } = useEncryption;
}