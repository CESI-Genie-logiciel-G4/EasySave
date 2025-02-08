using System.Buffers;
using EasySave.Helpers;

namespace EasySave.Services;

using Models;
using System.Text.Json;

public static class JobService
{
    public static List<BackupJob> BackupJobs { get; } = [];
    private static List<BackupJob> CompletedBackupJobs { get; set; } = [];
    private static DateTime LastLoadHistoricalBackupTime { get; set; } = DateTime.MinValue;
    public const int BackupJobLimit = 5;

    public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private const string BackupJobsFile = ".easysave/backup-jobs.json";
    private const string BackupJobsHistoricalFile = ".easysave/backup-jobs-historical.json";
    
    
    public static BackupJob AddBackupJob(string name, string source, string destination, BackupType type)
    {
        if (BackupJobs.Count >= BackupJobLimit)
        {
            throw new InvalidOperationException("Backup job limit reached");
        }
        
        var newJob = new BackupJob(name, source, destination, type);
        BackupJobs.Add(newJob);
        StoreJobs();
        
        return newJob;
    }
    
    public static void RemoveBackupJob(int index)
    {
        BackupJobs.RemoveAt(index);
        StoreJobs();
    }
    
    private static void StoreJobs()
    {
        var json = JsonSerializer.Serialize(BackupJobs, DefaultJsonOptions);
        FileHelper.CreateAndWrite(BackupJobsFile, json);
    }
    
    public static void LoadJobs()
    {
        if (!File.Exists(BackupJobsFile)) return;
        
        var json = File.ReadAllText(BackupJobsFile);
        var values = JsonSerializer.Deserialize<List<BackupJob>>(json);

        if (values is null) return;
            
        BackupJobs.AddRange(values);
    }

    public static void StoreCompletedBackupJob(BackupJob job)
    {
        List<BackupJob> completeBackupJobs;
        if (File.Exists(BackupJobsHistoricalFile))
        {
            var readJson = File.ReadAllText(BackupJobsHistoricalFile);
            completeBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(readJson)?? [];
        }
        else
        {
            completeBackupJobs = [];
        }
        
        completeBackupJobs.Add(job);
        var json = JsonSerializer.Serialize(completeBackupJobs, DefaultJsonOptions);
        FileHelper.CreateAndWrite(BackupJobsHistoricalFile, json);
    }

    public static void LoadCompletedBackupJob()
    {
        if (!File.Exists(BackupJobsHistoricalFile)) return;
        if (File.GetLastWriteTime(BackupJobsHistoricalFile) <= LastLoadHistoricalBackupTime) return;
        var readJson = File.ReadAllText(BackupJobsHistoricalFile);
        CompletedBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(readJson)?? [];
    }
    
    public static BackupJob? GetLastCompleteBackupJob(string jobSourcePath)
    {
        LoadCompletedBackupJob();
        return CompletedBackupJobs.LastOrDefault(job => job.SourceFolder == jobSourcePath && job.BackupType is FullBackup);
    }
}