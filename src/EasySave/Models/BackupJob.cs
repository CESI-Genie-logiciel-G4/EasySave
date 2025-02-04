namespace EasySave.Models;

public class BackupJob(string name)
{
    public string Name { get; } = name;
    
    public override string ToString()
    {
        return $"[BackupJob] {Name}";
    }
}