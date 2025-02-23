using System.Diagnostics;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Helpers;
using EasySave.Utils;
using static EasySave.Services.HistoryService;
using Timer = System.Threading.Timer;

namespace EasySave.Models;

public partial class Execution : ObservableObject, IDisposable
{
    public Execution(BackupJob backupJob)
    {
        BackupJob = backupJob;
        State = ExecutionState.Pending;
        Exception = null;
        _controller = new TaskController();
        
        _monitorTimer = new Timer(CheckBlockedProcesses, null, 0, 1000);
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
    private readonly TaskController _controller;
    
    [JsonIgnore]
    private readonly Timer _monitorTimer;
    
    public async Task Start()
    {
        ProgressUpdated?.Invoke(this);
        State = ExecutionState.Running;
        try 
        {
            await _controller.Start(Run);
        }
        finally
        {
            Dispose();
        }
    }
    
    public void Pause()
    {
        State = ExecutionState.Paused;
        _controller.Pause();
    }
    
    public void Resume()
    {
        State = ExecutionState.Running;
        _controller.Resume();
    }
    
    public void Cancel()
    {
        State = ExecutionState.Canceled;
        _controller.Cancel();
    }

    private void CheckBlockedProcesses(object? sender)
    {
        var shouldPause = BackupJob.BlockedProcesses
            .Any(blocked => Process.GetProcessesByName(blocked).Length > 0);
        
        switch (shouldPause)
        {
            case true when State == ExecutionState.Running:
                State = ExecutionState.Blocked;
                _controller.Pause();
                break;
            case false when State == ExecutionState.Blocked:
                Resume();
                break;
        }
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

    public void Dispose()
    {
        _controller.Dispose();
        _monitorTimer.Dispose();
    }
}