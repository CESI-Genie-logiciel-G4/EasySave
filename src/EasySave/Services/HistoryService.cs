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
        LoadHistory();
        CompletedExecutions.Add(execution);
        var json = JsonSerializer.Serialize(CompletedExecutions, DefaultJsonOptions);
        FileHelper.CreateAndWrite(ExecutionHistoryFile, json);
    }

    public static void LoadHistory()
    {
        if (!File.Exists(ExecutionHistoryFile)) return;
        if (LastHistoryLoad > File.GetLastWriteTimeUtc(ExecutionHistoryFile)) return;
        var readJson = File.ReadAllText(ExecutionHistoryFile);
        CompletedExecutions = JsonSerializer.Deserialize<List<Execution>>(readJson)?? [];
        LastHistoryLoad = DateTime.Now;
    }
    
    // Execution searcher
    private static Execution? GetLastFullBackupJobExecution(string jobSourcePath)
    {
        return CompletedExecutions.LastOrDefault(
            execution =>
                execution.BackupJob.SourceFolder == jobSourcePath 
                && execution.BackupJob.BackupType is FullBackup or SyntheticFullBackup
        );
    }
    
    private static Execution? GetLastBackupExecutionForFolder(string sourceFolder)
    {
        return CompletedExecutions.LastOrDefault(
            execution => execution.BackupJob.SourceFolder == sourceFolder
        );
    }

    private static Execution? GetLastBackupJobExecution(BackupJob backupJob)
    {
        return CompletedExecutions.LastOrDefault(
            execution =>
                execution.BackupJob.Name == backupJob.Name 
                &&  execution.BackupJob.SourceFolder == backupJob.SourceFolder
                &&  execution.BackupJob.DestinationFolder == backupJob.DestinationFolder
                &&  execution.BackupJob.BackupType == backupJob.BackupType
        );
    }

    // Specific searched data
    public static string? GetLastCompleteBackupFolder(string sourceFolder)
    {
        return GetLastFullBackupJobExecution(sourceFolder)?.BackupJob.DestinationFolder;
    }

    public static DateTime? GetLastBackupExecutionDate(string sourceFolder)
    {
        var lastBackupExecution = CompletedExecutions.LastOrDefault(
            execution =>
                execution.BackupJob.SourceFolder == sourceFolder
        );
        return lastBackupExecution?.StartTime;
    }
    
    public static DateTime? GetLastBackupJobExecutionDate(BackupJob backupJob)
    {
        return GetLastBackupJobExecution(backupJob)?.StartTime;
    }

    public static bool IsLastExecuteBackupJob(BackupJob backupJob)
    {
        return backupJob == GetLastBackupExecutionForFolder(backupJob.SourceFolder)?.BackupJob;
    }

    public static bool CanCreateDifferentialBackup(string sourceFolder)
    {
        return GetLastFullBackupJobExecution(sourceFolder) != null;
    }

}