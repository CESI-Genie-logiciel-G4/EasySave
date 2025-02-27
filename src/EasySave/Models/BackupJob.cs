using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Models.Backups;
using EasySave.Services;

namespace EasySave.Models;

public partial class BackupJob : ObservableObject
{
    public string Name { get; }
    public string SourceFolder { get; }
    public string DestinationFolder { get; }
    public BackupType BackupType { get; }
    public bool UseEncryption { get; }
    
    public ObservableCollection<string> BlockedProcesses { get; } = SettingsService.Settings.PriorityProcessNames;
    
    [ObservableProperty]
    [property: JsonIgnore]
    [JsonIgnore]
    private Execution? _currentExecution;

    public BackupJob(
        string name, 
        string sourceFolder, 
        string destinationFolder, 
        BackupType backupType,
        bool useEncryption
        ) 
    {
        Name = name;
        SourceFolder = sourceFolder;
        DestinationFolder = destinationFolder;
        BackupType = backupType;
        UseEncryption = useEncryption;
    }
    
    public Execution InitializeExecution()
    {
        if (CurrentExecution?.State is ExecutionState.Running)
            throw new InvalidOperationException("An execution is already running for this job");

        CurrentExecution?.Dispose();

        CurrentExecution = new Execution(this);
        return CurrentExecution;
    }

    public async Task ExecuteAsync()
    {
        if (CurrentExecution?.State is not ExecutionState.Pending)
            throw new InvalidOperationException("Execution isn't in a pending state");

        await CurrentExecution.Start();
    }
}