using EasySave.Models;
using EasySave.Models.Backups;
using EasySave.Services;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.ViewModels;

public class MainViewModel
{
    private readonly JobService _jobService;
    private readonly List<BackupJob> _backupJobs ;
    private readonly LocalizationService _localizationService;
    private readonly Logger.Logger _logger;
    
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
    
    public MainViewModel(JobService jobService, LocalizationService localizationService, Logger.Logger logger)
    {
        _jobService = jobService;
        _backupJobs = _jobService.BackupJobs;
        _localizationService = localizationService;
        _logger = logger;
        
        _logger.SetupTransporters(ExtractLogsTransporters());
    }

    public event Action<BackupJob>? BackupJobAdded;
    public event Action<int>? BackupJobRemoved;
    public event Action<Execution>? ProgressUpdated;
    public event Action<Exception>? ErrorOccurred;
    public event Action? LogsTransportersChanged;

    public void AddBackupJob(string name, string source, string destination, BackupType type)
    {
        var newJob = _jobService.AddBackupJob(name, source, destination, type);
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
        _jobService.RemoveBackupJob(index);
        BackupJobRemoved?.Invoke(index);
    }

    public void ChangeLanguage(LanguageItem language)
    {
        _localizationService.SetLanguage(language.Identifier);
    }
    
    public void ChangeLogsTransporters(List<int> indexes)
    {
        foreach (var numericalIndex in indexes)
        {
            var transporter = LogTransporters[numericalIndex - 1];
            transporter.IsEnabled = !transporter.IsEnabled;
        }
        
        _logger.SetupTransporters(ExtractLogsTransporters());
        
        LogsTransportersChanged?.Invoke();
    }
    
    private List<Transporter> ExtractLogsTransporters()
    {
        return LogTransporters.Where(t => t.IsEnabled)
            .Select(t => t.Transporter)
            .ToList();
    }
}