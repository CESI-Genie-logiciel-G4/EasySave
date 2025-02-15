using System.Text.Json;
using EasySave.Helpers;
using EasySave.Models;
using EasySave.Models.Backups;

namespace EasySave.Services;

public static class HistoryService
{
    private static List<Execution> CompletedExecutions { get; set; } = [];
    private static DateTime LastHistoryLoad { get; set; } = DateTime.MinValue;
    private const string ExecutionHistoryFile = ".easysave/executions-history.json";

    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    
    
    public static void StoreCompletedExecution(Execution execution)
    {
        List<Execution> completedExecutions;
        if (File.Exists(ExecutionHistoryFile))
        {
            var readJson = File.ReadAllText(ExecutionHistoryFile);
            completedExecutions = JsonSerializer.Deserialize<List<Execution>>(readJson)?? [];
        }
        else
        {
            completedExecutions = [];
        }
        
        completedExecutions.Add(execution);
        var json = JsonSerializer.Serialize(completedExecutions, DefaultJsonOptions);
        FileHelper.CreateAndWrite(ExecutionHistoryFile, json);
    }

    public static void LoadHistory()
    {
        if (!File.Exists(ExecutionHistoryFile)) return;
        if (File.GetLastWriteTime(ExecutionHistoryFile) <= LastHistoryLoad) return;
        var readJson = File.ReadAllText(ExecutionHistoryFile);
        CompletedExecutions = JsonSerializer.Deserialize<List<Execution>>(readJson)?? [];
        LastHistoryLoad = DateTime.Now;
    }
    
    public static BackupJob? GetLastCompleteBackupJob(string jobSourcePath)
    {
        LoadHistory();
        var lastFullBackupExecution = CompletedExecutions.LastOrDefault(
            execution =>
                execution.BackupJob.SourceFolder == jobSourcePath 
                && execution.BackupJob.BackupType is FullBackup or SyntheticFullBackup
            );
        return lastFullBackupExecution?.BackupJob;
    }
}