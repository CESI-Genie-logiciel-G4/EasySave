using CommunityToolkit.Mvvm.ComponentModel;
using EasySave.Helpers;
using static EasySave.Services.HistoryService;

namespace EasySave.Models;

public partial class Execution : ObservableObject
{
    public Execution(BackupJob backupJob)
    {
        BackupJob = backupJob ?? throw new ArgumentNullException(nameof(backupJob));
        State = ExecutionState.Pending;
        Exception = null;
    }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public BackupJob BackupJob { get; }

    public event Action<Execution>? ProgressUpdated;

    [ObservableProperty] private ExecutionState state;

    [ObservableProperty] private Exception? exception;

    [ObservableProperty] private int currentProgress;

    [ObservableProperty] private int totalSteps;

    public void Run()
    {
        StartTime = DateTime.UtcNow;
        ProgressUpdated?.Invoke(this);
        State = ExecutionState.Running;

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
                var destinationFile = FileHelper.GetMirrorFilePath(rootFolder, sourceFile, destinationFolder);
                BackupJob.BackupType.Execute(sourceFile, destinationFile, BackupJob);
                CurrentProgress++;
                ProgressUpdated?.Invoke(this);
            }
            
            State = ExecutionState.Completed;

            StoreCompletedExecution(this);
            ProgressUpdated?.Invoke(this);
        }
        catch (Exception e)
        {
            State = ExecutionState.Failed;
            Exception = e;
        }

        EndTime = DateTime.UtcNow;
    }
}