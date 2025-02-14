using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models;

public class BackupJob(string name, string sourceFolder, string destinationFolder, BackupType backupType)
{
    public string Name { get; } = name;
    public string SourceFolder { get; } = sourceFolder;
    public string DestinationFolder { get; } = destinationFolder;
    public BackupType BackupType { get; } = backupType;
}