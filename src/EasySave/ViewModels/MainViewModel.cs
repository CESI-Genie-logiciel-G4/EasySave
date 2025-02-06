using EasySave.Models;
using EasySave.Services;
using EasySave.Utils;

namespace EasySave.ViewModels;

public class MainViewModel
{
    public List<BackupJob> BackupJobs { get; } = [
        new("Documents", @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Source", @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Directory", new FullBackup()),
        new("Images", @"C:\Users\John\Images", @"D:\Backups\Images", new FullBackup()),
        new("Videos", @"C:\Users\John\Videos", @"D:\Backups\Videos", new DifferentialBackup())
    ];
    
    public List<LanguageItem> Languages { get; } = [
        new("English", "en"),
        new("French", "fr")
    ];
    
    public event Action<BackupJob>? BackupJobAdded;
    public event Action<BackupJob>? BackupJobExecuted;
    
    public event Action<int>? BackupJobRemoved; 
    
    public void AddBackupJob(string name, string source, string destination, BackupType type)
    {
        var newJob = new BackupJob(name, source, destination, type);
        BackupJobs.Add(newJob);
        
        BackupJobAdded?.Invoke(newJob);
    }
    
    public void ExecuteJob(int index)
    {
        // var job = BackupJobs[index];

        var source = @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Source";
        var destination = @"D:\Brieuc\CESI\A3 FISA INFO\Génie Logiciel\ProjetTest\Directory";

        var job = new BackupJob("JobTest", source, destination, new DifferentialBackup());
        
        job.Run();
        
        BackupJobExecuted?.Invoke(job);
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