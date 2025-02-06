namespace EasySave.Models;

public class FullBackup : BackupType
{
    public override void Execute(string sourceFile, string destinationFile, Execution execution)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
        File.Copy(sourceFile, destinationFile, true);
        execution.UpdateProgress(10);
    }
}
