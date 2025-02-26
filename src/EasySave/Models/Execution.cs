using System.Diagnostics;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Helpers;
using EasySave.Services;
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
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [JsonIgnore] public int ElapsedTime => (int)(EndTime - StartTime).TotalSeconds;
    public BackupJob BackupJob { get; }

    public event Action<Execution>? ProgressUpdated;

    [ObservableProperty] private ExecutionState _state;

    [ObservableProperty] private Exception? _exception;

    [ObservableProperty] private int _currentProgress;

    [ObservableProperty] private int _totalSteps;
    
    [JsonIgnore] private readonly TaskController _controller;
    
    [JsonIgnore] private static  readonly Semaphore LargeFileSemaphore = new (1,1);
    
    public async Task Start()
    {
        ProgressUpdated?.Invoke(this);
        State = ExecutionState.Running;

        await using var monitorTimer = new Timer(CheckBlockedProcesses, null, 0, 1000);

        try
        {
            await _controller.Start(Run);
        }
        finally
        {
            await monitorTimer.DisposeAsync();
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

    private void ExecuteBackupForFile(string sourceFile, string destinationFile)
    {
        if (IsOverMaxSize(sourceFile))
        {
            if (!LargeFileSemaphore.WaitOne(0))
            {
                State = ExecutionState.Waiting;
                LargeFileSemaphore.WaitOne();
                
                if (State == ExecutionState.Waiting) 
                    State = ExecutionState.Running;
            }
            
            try
            {
                BackupJob.BackupType.Execute(sourceFile, destinationFile, BackupJob);
            }
            finally
            {
                LargeFileSemaphore.Release();
            }
        }
        else
        {
            BackupJob.BackupType.Execute(sourceFile, destinationFile, BackupJob);
        }
    }

    private bool IsOverMaxSize(string file)
    {
        return FileHelper.GetFileSize(file) > SettingsService.Settings.MaxFileSize;
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
                ExecuteBackupForFile(sourceFile, destinationFile);
                CurrentProgress++;
                ProgressUpdated?.Invoke(this);
            }

            State = ExecutionState.Completed;

            EndTime = DateTime.UtcNow;
            StoreCompletedExecution(this);
            ProgressUpdated?.Invoke(this);
        }
        catch (OperationCanceledException)
        {
            State = ExecutionState.Canceled;
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
        GC.SuppressFinalize(this);
    }
}