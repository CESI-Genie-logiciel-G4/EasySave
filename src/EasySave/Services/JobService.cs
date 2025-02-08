using System.Buffers;
using EasySave.Helpers;

namespace EasySave.Services;

using Models;
using System.Text.Json;

public static class JobService
{
    public static List<BackupJob> BackupJobs { get; } = [];
    public const int BackupJobLimit = 5;
    
    public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private const string BackupJobsFile = ".easysave/backup-jobs.json";
    private const string BackupJobsHistoryFile = ".easysave/complete-backup-jobs.json";
    
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

    public static void StoreNewBackupJob(BackupJob job)
    {
        List<BackupJob> completeBackupJobs;
        if (File.Exists(BackupJobsHistoryFile))
        {
            var readJson = File.ReadAllText(BackupJobsHistoryFile);
            completeBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(readJson)?? [];
        }
        else
        {
            completeBackupJobs = [];
        }
        
        completeBackupJobs.Add(job);
        var json = JsonSerializer.Serialize(completeBackupJobs, DefaultJsonOptions);
        FileHelper.CreateAndWrite(BackupJobsHistoryFile, json);
    }

    public static BackupJob? LoadLastCompleteBackupJob(string jobSourcePath)
    {
        if (!File.Exists(BackupJobsHistoryFile)) return null;
        
        var readJson = File.ReadAllText(BackupJobsHistoryFile);
        var completeBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(readJson)?? [];
        return completeBackupJobs.LastOrDefault(job => job.SourceFolder == jobSourcePath && job.BackupType is FullBackup);
    }
}