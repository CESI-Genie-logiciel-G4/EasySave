namespace EasySave.ViewModels;

using Models;

public class MainViewModel
{
    public string Title { get; }
    public List<BackupJob> BackupJobs { get; }
    
    public event Action<BackupJob>? BackupJobAdded;

    public MainViewModel()
    {
        Title = "Gestion des Backups";
        
        BackupJobs = 
        [
            new BackupJob("Full"),
            new BackupJob("Differential"),
        ];
    }
    
    public void AddBackupJob(string name)
    {
        var newJob = new BackupJob(name);
        BackupJobs.Add(newJob);
        
        BackupJobAdded?.Invoke(newJob);
    }
}