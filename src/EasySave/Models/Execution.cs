namespace EasySave.Models;

using Helpers;
using static Services.HistoryService;

public class Execution(BackupJob backupJob)
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ExecutionState State { get; set; } = ExecutionState.Pending;
    public Exception? Exception { get; set; } = null;
    public BackupJob BackupJob { get; } = backupJob;
    public event Action<Execution>? ProgressUpdated;
    public int CurrentProgress { get; set; } = 0;
    public int TotalSteps { get; set; }
    
    public void Run()
    {
        StartTime = DateTime.UtcNow;
        ProgressUpdated?.Invoke(this);
        State = ExecutionState.Running;
        
        var rootFolder = BackupJob.SourceFolder;
        var destinationFolder = BackupJob.DestinationFolder;
        
        var files = Directory.GetFiles(rootFolder, "*", SearchOption.AllDirectories);
        TotalSteps = files.Length;
        
        ProgressUpdated?.Invoke(this);
        
        try
        {
            BackupJob.BackupType.Initialize(BackupJob);
            
            foreach (var sourceFile in files)
            {
                var destinationFile = FileHelper.GetMirrorFilePath(rootFolder, sourceFile, destinationFolder);
                BackupJob.BackupType.Execute(sourceFile, destinationFile, BackupJob);
                CurrentProgress++;
                ProgressUpdated?.Invoke(this);
            }
            
            State = ExecutionState.Completed;
            ProgressUpdated?.Invoke(this);
        }
        catch (Exception e)
        {
            State = ExecutionState.Failed;
            Exception = e;
        }
        EndTime = DateTime.UtcNow;
        StoreCompletedExecution(this);
    }
}