using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Exceptions;
using EasySave.Models;
using EasySave.Models.Backups;
using EasySave.Services;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public readonly ObservableCollection<string> EncryptedExtensions = ExtensionService.EncryptedExtensions;
    private static string T(string key) => LocalizationService.GetString(key);
    
    [ObservableProperty]
    private ObservableCollection<BackupJob> _backupJobs;

    [ObservableProperty]
    private ObservableCollection<Execution> _history;
    
    public MainViewModel()
    {
        BackupJobs = new(JobService.BackupJobs);
        History = new(HistoryService.CompletedExecutions);
        
        Logger.Logger.GetInstance()
            .SetupTransporters(ExtractLogsTransporters());
    }
    
    public List<LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("French", "fr")
    ];

    public List<BackupType> BackupTypes { get; } =
    [
        new FullBackup(),
        new SyntheticFullBackup(),
        new DifferentialBackup()
    ];
    
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
    public event Action<List<string>>? EncryptedExtensionsChanged;
    public event Action<string>? ExtensionsAdded;
    public event Action<string>? ExtensionsRemoved;
    public event Action<string>? ExtensionsAlreadyExists;

    public void AddBackupJob(string name, string source, string destination, BackupType type, bool encryption)
    {
        var newJob = JobService.AddBackupJob(name, source, destination, type, encryption);
        BackupJobAdded?.Invoke(newJob);
    }

    public async void ExecuteJob(int index)
    {
        var job = BackupJobs[index];
        var execution = new Execution(job);
        
        execution.ProgressUpdated += (e) =>
        {
            ProgressUpdated?.Invoke(e);
        };
        
        await Task.Run(() => execution.Run());
        
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
    
    public void AddExtensions(string extension)
    {
        try
        {
            var newExtension = ExtensionService.AddEncryptedExtension(extension);
            if (newExtension == null)
            {
                ExtensionsAlreadyExists?.Invoke(extension);
                return;
            }

            ExtensionsAdded?.Invoke(newExtension);
        }
        catch (AlreadyExistException e)
        {
            ExtensionsAlreadyExists?.Invoke(e.Extension);
        }
        
    }

    public void RemoveExtension(List<int> indexes)
    {
        foreach (var index in indexes.OrderByDescending(i => i))
        {
            var extension = ExtensionService.EncryptedExtensions[index - 1];
            ExtensionService.RemoveEncryptedExtension(extension);
            ExtensionsRemoved?.Invoke(extension);
        }
    }
}