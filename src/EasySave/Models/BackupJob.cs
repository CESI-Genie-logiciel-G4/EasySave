namespace EasySave.Models;

public class BackupJob(string name, string sourceFolder, string destinationFolder, BackupType backupType)
{
    public string Name { get; } = name;

    public void Run()
    {
        string[] files = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories);
        Execution execution = new Execution(files.Length * 10);
        execution.ProgressUpdated += (progress) => Console.WriteLine($"Progress: {progress}%");

        Console.WriteLine($"Starting backup from {sourceFolder} to {destinationFolder}...");

        foreach (string sourceFile in files)
        {
            string relativePath = Path.GetRelativePath(sourceFolder, sourceFile);
            string destinationFile = Path.Combine(destinationFolder, relativePath);

            backupType.Execute(sourceFile, destinationFile, execution);
        }

        Console.WriteLine("Backup completed successfully!");
    }
}