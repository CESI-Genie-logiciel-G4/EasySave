using EasySave.Models;
using EasySave.Models.Backups;
using EasySave.Services;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.ViewModels;

public class MainViewModel
{
    private readonly List<BackupJob> _backupJobs = JobService.BackupJobs;
    private static string T(string key) => LocalizationService.GetString(key);

    public MainViewModel()
    {
        Logger.Logger.GetInstance()
            .SetupTransporters(ExtractLogsTransporters());
    }
    
    public List<LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("French", "fr")
    ];

    public Dictionary<string, BackupType> BackupTypes { get; } = new()
    {
        ["Full"] = new FullBackup(),
        ["Synthetic-Full"] = new SyntheticFullBackup(),
        ["Differential"] = new DifferentialBackup(),
    };
    
    public List<TransporterItem> LogTransporters { get; } =
    [
        new("Console", new ConsoleTransporter(), false),
        new("XML", new FileXmlTransporter("./.easysave/logs/")),
        new("JSON", new FileJsonTransporter("./.easysave/logs/"))
    ];

    public event Action<BackupJob>? BackupJobAdded;
    public event Action<int>? BackupJobRemoved;
    public event Action<Execution>? ProgressUpdated;
    public event Action<Exception>? ErrorOccurred;
    public event Action? LogsTransportersChanged;

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
    
    public void ChangeLogsTransporters(List<int> indexes)
    {
        foreach (var numericalIndex in indexes)
        {
            var transporter = LogTransporters[numericalIndex - 1];
            transporter.IsEnabled = !transporter.IsEnabled;
        }
        
        Logger.Logger.GetInstance()
            .SetupTransporters(ExtractLogsTransporters());
        
        LogsTransportersChanged?.Invoke();
    }
    
    private List<Transporter> ExtractLogsTransporters()
    {
        return LogTransporters.Where(t => t.IsEnabled)
            .Select(t => t.Transporter)
            .ToList();
    }
}