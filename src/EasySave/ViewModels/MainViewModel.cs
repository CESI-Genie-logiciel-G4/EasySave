using EasySave.Models;

namespace EasySave.ViewModels;

public class MainViewModel
{
    public List<BackupJob> BackupJobs { get; } = [
        new("Documents"),
        new("Images"),
        new("Videos")
    ];
    
    public event Action<BackupJob>? BackupJobAdded;
    public event Action<BackupJob>? BackupJobExecuted;
    
    public event Action<int>? BackupJobRemoved; 
    
    public void AddBackupJob(string name)
    {
        var newJob = new BackupJob(name);
        BackupJobs.Add(newJob);
        
        BackupJobAdded?.Invoke(newJob);
    }
    
    public void ExecuteJob(int index)
    {
        var job = BackupJobs[index];
        Thread.Sleep(1000);
        BackupJobExecuted?.Invoke(job);
    }
    
    public void RemoveJob(int index)
    {
        BackupJobs.RemoveAt(index);
        BackupJobRemoved?.Invoke(index);
    }
}