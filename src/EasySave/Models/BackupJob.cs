namespace EasySave.Models;

public class BackupJob(string name, string sourceFolder, string destinationFolder, BackupType backupType)
{
    public string Name { get; } = name;

    public string SourceFolder { get; } = sourceFolder;
    public string DestinationFolder { get; } = destinationFolder;

    public void Run()
    {
        string[] files = Directory.GetFiles(SourceFolder, "*", SearchOption.AllDirectories);
        Execution execution = new Execution(files.Length * 10);
        execution.ProgressUpdated += (progress) => Console.WriteLine($"Progress: {progress}%");

        Console.WriteLine($"Starting backup from {SourceFolder} to {DestinationFolder}...");

        foreach (string sourceFile in files)
        {
            string relativePath = Path.GetRelativePath(SourceFolder, sourceFile);
            string destinationFile = Path.Combine(DestinationFolder, relativePath);

            backupType.Execute(sourceFile, destinationFile, execution, this);
        }

        Console.WriteLine("Backup completed successfully!");
    }
}