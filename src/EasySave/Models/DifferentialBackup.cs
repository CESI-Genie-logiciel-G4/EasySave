namespace EasySave.Models;

public class DifferentialBackup : BackupType
{
    public override void Execute(string sourceFile, string destinationFile, Execution execution)
    {
        if (!File.Exists(destinationFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(destinationFile))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(sourceFile, destinationFile, true);
            Console.WriteLine($"Copied: {sourceFile} to {destinationFile}");
            execution.UpdateProgress(10);
        }
    }
}