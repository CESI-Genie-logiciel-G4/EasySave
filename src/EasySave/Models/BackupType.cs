namespace EasySave.Models;

public abstract class BackupType
{
    public abstract void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job);
}