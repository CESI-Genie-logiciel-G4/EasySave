using EasySave.Models;
using EasySave.Services;
using EasySave.Utils;

namespace EasySave.ViewModels;

public class MainViewModel
{
    private readonly List<BackupJob> _backupJobs = JobService.BackupJobs;
    private static string T(string key) => LocalizationService.GetString(key);

    public List<LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("French", "fr")
    ];

    public Dictionary<string, BackupType> BackupTypes { get; } = new()
    {
        ["Full"] = new FullBackup(),
        ["Differential"] = new DifferentialBackup()
    };

    public event Action<BackupJob>? BackupJobAdded;
    public event Action<int>? BackupJobRemoved;
    public event Action<Execution>? ProgressUpdated;
    
    public event Action<Exception>? ErrorOccurred;
    
    public void AddBackupJob(string name, string source, string destination, BackupType type)
    {
        var newJob = JobService.AddBackupJob(name, source, destination, type);
        BackupJobAdded?.Invoke(newJob);
    }

    public void ExecuteJob(int index)
    {
        var job = _backupJobs[index];
        var execution = new Execution(job);
        
        execution.ProgressUpdated += (e) => ProgressUpdated?.Invoke(e);
        execution.Run();
        
        if(execution.State == ExecutionState.Failed)
        {
            ErrorOccurred?.Invoke(execution.Exception!);
        }
    }

    public void RemoveJob(int index)
    {
        JobService.RemoveBackupJob(index);
        BackupJobRemoved?.Invoke(index);
    }

    public void ChangeLanguage(LanguageItem language)
    {
        LocalizationService.SetLanguage(language.Identifier);
    }
}