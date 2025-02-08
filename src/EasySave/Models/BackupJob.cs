using EasySave.Helpers;
using EasySave.Services;

namespace EasySave.Models;

public class BackupJob(string name, string sourceFolder, string destinationFolder, BackupType backupType)
{
    public string Name { get; } = name;
    public string SourceFolder { get; } = sourceFolder;
    public string DestinationFolder { get; } = destinationFolder;
    public BackupType BackupType { get; } = backupType;

    public void Run(Execution execution)
    {
        var files = Directory.GetFiles(SourceFolder, "*", SearchOption.AllDirectories);
        execution.SetTotalSteps(files.Length);

        foreach (var sourceFile in files)
        {
            var destinationFile = FileHelper.GetMirrorFilePath(sourceFolder, sourceFile, destinationFolder);

            BackupType.Execute(sourceFile, destinationFile, execution, this);
        }
        JobService.StoreNewBackupJob(this);
    }
}