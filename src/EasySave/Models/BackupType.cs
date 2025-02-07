namespace EasySave.Models;

public abstract class BackupType(string name)
{
    public string Name { get; } = name;
    public abstract void Execute(string sourceFile, string destinationFile, Execution execution, BackupJob job);
}