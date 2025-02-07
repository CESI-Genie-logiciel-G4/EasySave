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
            var relativePath = Path.GetRelativePath(SourceFolder, sourceFile);
            var destinationFile = Path.Combine(DestinationFolder, relativePath);

            BackupType.Execute(sourceFile, destinationFile, execution, this);
        }
    }
}