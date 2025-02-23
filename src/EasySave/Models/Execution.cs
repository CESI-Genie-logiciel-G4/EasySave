using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Helpers;
using EasySave.Utils;
using static EasySave.Services.HistoryService;

namespace EasySave.Models;

public partial class Execution : ObservableObject
{
    public Execution(BackupJob backupJob)
    {
        BackupJob = backupJob;
        State = ExecutionState.Pending;
        Exception = null;
        _controller = new TaskController();
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BackupJob BackupJob { get; }

    public event Action<Execution>? ProgressUpdated;

    [ObservableProperty] 
    private ExecutionState _state;

    [ObservableProperty] 
    private Exception? _exception;

    [ObservableProperty] 
    private int _currentProgress;

    [ObservableProperty] 
    private int _totalSteps;
    
    [JsonIgnore]
    [property: JsonIgnore]
    [ObservableProperty]
    private TaskController _controller;
    
    public async Task Start()
    {
        ProgressUpdated?.Invoke(this);
        State = ExecutionState.Running;
        await Controller.Start(Run);
    }
    
    public void Pause()
    {
        State = ExecutionState.Paused;
        Controller.Pause();
    }
    
    public void Resume()
    {
        State = ExecutionState.Running;
        Controller.Resume();
    }
    
    public void Cancel()
    {
        State = ExecutionState.Canceled;
        Controller.Cancel();
    }

    private Task Run(CancellationToken token, ManualResetEventSlim pauseEvent)
    {
        StartTime = DateTime.UtcNow;
        try
        {
            var rootFolder = BackupJob.SourceFolder;
            var destinationFolder = BackupJob.DestinationFolder;
            var files = Directory.GetFiles(rootFolder, "*", SearchOption.AllDirectories);

            TotalSteps = files.Length;
            ProgressUpdated?.Invoke(this);

            BackupJob.BackupType.Initialize(BackupJob);

            foreach (var sourceFile in files)
            {
                token.ThrowIfCancellationRequested();
                pauseEvent.Wait(token);
                Thread.Sleep(1000);
                var destinationFile = FileHelper.GetMirrorFilePath(rootFolder, sourceFile, destinationFolder);
                BackupJob.BackupType.Execute(sourceFile, destinationFile, BackupJob);
                CurrentProgress++;
                ProgressUpdated?.Invoke(this);
            }
            
            State = ExecutionState.Completed;

            EndTime = DateTime.UtcNow;
            StoreCompletedExecution(this);
            ProgressUpdated?.Invoke(this);
        }
        catch (Exception e)
        {
            State = ExecutionState.Failed;
            Exception = e;
        }

        return Task.CompletedTask;
    }
}