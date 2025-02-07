using EasySave.Models;
using EasySave.Services;
using EasySave.Utils;

namespace EasySave.ViewModels;

public class MainViewModel
{
    private readonly List<BackupJob> _backupJobs = JobService.BackupJobs;
    private static string T(string key) => LocalizationService.GetString(key);
    
    public List<LanguageItem> Languages { get; } = [
        new("English", "en"),
        new("French", "fr")
    ];
    
    public Dictionary<string, BackupType> BackupTypes { get; } = new()
    {
        ["Full"] = new FullBackup(),
        ["Differential"] = new DifferentialBackup()
    };

    public event Action<BackupJob>? BackupJobAdded;
    public event Action<BackupJob>? BackupJobExecuted;
    public event Action<int>? BackupJobRemoved; 
    public event Action<string, bool>? Notification; 
    public event Action<int, int>? ProgressUpdated; 
    
    public void AddBackupJob(string name, string source, string destination, BackupType type)
    {
        var newJob = JobService.AddBackupJob(name, source, destination, type);
        BackupJobAdded?.Invoke(newJob);
    }
    
    public void ExecuteJob(int index)
    {
        var execution = new Execution();
        execution.Notifier += (message, isError) => Notification?.Invoke(message, isError);
        execution.ProgressUpdated += (current, total) => ProgressUpdated?.Invoke(current, total);
        
        var job = _backupJobs[index];
        
        execution.SetMessage($"{T("StartBackupJob")} {job.Name} [{job.SourceFolder} -> {job.DestinationFolder}]");

        try
        {
            job.Run(execution);
            BackupJobExecuted?.Invoke(job);
        }

        catch (DirectoryNotFoundException)
        {
            execution.SetError(T("DirectoryNotFound"));
        }
        catch (UnauthorizedAccessException)
        {
            execution.SetError(T("UnauthorizedAccess"));
        }
        catch (Exception)
        {
            execution.SetError(T("ErrorOccurred"));
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