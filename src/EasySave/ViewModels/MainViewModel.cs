using EasySave.Models;
using EasySave.Services;
using EasySave.Utils;

namespace EasySave.ViewModels;

public class MainViewModel
{
    private static string T(string key) => LocalizationService.GetString(key);
    public List<BackupJob> BackupJobs { get; } = [
        new("Documents", @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Source", @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Directory", new FullBackup()),
        new("Images", @"C:\Users\John\Images", @"D:\Backups\Images", new FullBackup()),
        new("Videos", @"C:\Users\John\Videos", @"D:\Backups\Videos", new DifferentialBackup())
    ];
    
    public List<LanguageItem> Languages { get; } = [
        new("English", "en"),
        new("French", "fr")
    ];
    
    public Dictionary<string, BackupType> BackupTypes { get; } = new()
    {
        ["Full"] = new FullBackup(),
        ["Differential"] = new DifferentialBackup()
    };

    public const int BackupJobLimit = 5;

    public event Action<BackupJob>? BackupJobAdded;
    public event Action<BackupJob>? BackupJobExecuted;
    public event Action<int>? BackupJobRemoved; 
    public event Action<string, bool>? Notification; 
    public event Action<int, int>? ProgressUpdated; 
    
    public void AddBackupJob(string name, string source, string destination, BackupType type)
    {
        if (BackupJobs.Count >= BackupJobLimit)
        {
            throw new InvalidOperationException("Backup job limit reached");
        }
        
        var newJob = new BackupJob(name, source, destination, type);
        BackupJobs.Add(newJob);
        
        BackupJobAdded?.Invoke(newJob);
    }
    
    public void ExecuteJob(int index)
    {
        var execution = new Execution();
        execution.Notifier += (message, isError) => Notification?.Invoke(message, isError);
        execution.ProgressUpdated += (current, total) => ProgressUpdated?.Invoke(current, total);
        
        var job = BackupJobs[index];
        
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
        BackupJobs.RemoveAt(index);
        BackupJobRemoved?.Invoke(index);
    }
    
    public void ChangeLanguage(LanguageItem language)
    {
        LocalizationService.SetLanguage(language.Identifier);
    }
}