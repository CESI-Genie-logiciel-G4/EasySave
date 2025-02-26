using System.Collections.ObjectModel;
using System.Text.Json;
using DynamicData;
using EasySave.Exceptions;
using EasySave.Helpers;
using EasySave.Models;
using EasySave.Models.Backups;

namespace EasySave.Services;

public static class HistoryService
{
    public static ObservableCollection<Execution> CompletedExecutions { get; } = [];
    private const string ExecutionHistoryFile = ".easysave/executions-history.json";

    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        { WriteIndented = true };

    public static void StoreCompletedExecution(Execution execution)
    {
        CompletedExecutions.Add(execution);
        var json = JsonSerializer.Serialize(CompletedExecutions, DefaultJsonOptions);
        FileHelper.CreateAndWrite(ExecutionHistoryFile, json);
    }

    public static void LoadHistory()
    {
        if (!File.Exists(ExecutionHistoryFile))
            return;
        var readJson = File.ReadAllText(ExecutionHistoryFile);

        CompletedExecutions.Clear();
        CompletedExecutions.AddRange(JsonSerializer.Deserialize<List<Execution>>(readJson) ?? []);
    }

    private static BackupJob GetLastCompleteBackupJob(string jobSourceGlobalPath)
    {
        var cleanedJobSourcePath = Path.TrimEndingDirectorySeparator(jobSourceGlobalPath);
        
        var lastFullBackupExecution = CompletedExecutions.LastOrDefault(
            execution => Path.TrimEndingDirectorySeparator(execution.BackupJob.SourceFolder)
                             .Equals(cleanedJobSourcePath) 
                             && execution.BackupJob.BackupType is FullBackup or SyntheticFullBackup);
        
        if (lastFullBackupExecution is null)
        {
            throw new MissingFullBackupException();
        }
        
        return lastFullBackupExecution.BackupJob;
    }

    public static string GetLastCompleteBackupFolder(string sourceFolder)
    {
        var lastFullBackupJob = GetLastCompleteBackupJob(sourceFolder);
        
        if (!Directory.Exists(lastFullBackupJob.DestinationFolder))
        {
            throw new MissingFullBackupException();
        }
        
        return lastFullBackupJob.DestinationFolder;
    }
}