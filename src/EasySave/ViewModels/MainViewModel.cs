using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Converters;
using EasySave.Exceptions;
using EasySave.Models;
using EasySave.Models.Backups;
using EasySave.Services;
using EasySave.Utils;
using Logger.Transporters;

namespace EasySave.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> _encryptedExtensions = ExtensionService.EncryptedExtensions;

    [ObservableProperty] private ObservableCollection<BackupJob> _backupJobs;

    [ObservableProperty] private ObservableCollection<Execution> _history;

    [ObservableProperty] private ObservableCollection<string> _businessApps = [];

    [ObservableProperty] private ObservableCollection<string> _priorityFiles = [];

    [ObservableProperty] private ObservableCollection<UiType> _usageMode = [UiType.Console, UiType.Gui];

    public MainViewModel()
    {
        BackupJobs = JobService.BackupJobs;
        History = HistoryService.CompletedExecutions;

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

    [ObservableProperty] private ObservableCollection<TransporterItem> _logTransporters = SettingsService.Settings.LogTransporters;

    public long FileSizeValue
    {
        get => SettingsService.Settings.MaxFileSizeKb;
        set
        {
            if (value != SettingsService.Settings.MaxFileSizeKb)
            {
                SettingsService.UpdateMaxFileSize(value);
            }
        }
    }

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

    public async Task ExecuteJob(int index)
    {
        var job = BackupJobs[index];
        var execution = job.InitializeExecution();

        execution.ProgressUpdated += (e) => { ProgressUpdated?.Invoke(e); };

        await job.ExecuteAsync();

        if (execution.State == ExecutionState.Failed)
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
        LocalizationService.UpdateAvaloniaResources();
    }

    public void ChangeLogsTransporters(List<int> indexes)
    {
        SettingsService.UpdateLogTransporters(indexes);
        
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