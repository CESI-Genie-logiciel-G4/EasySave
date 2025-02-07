using EasySave.Helpers;

namespace EasySave.Services;

using Models;
using System.Text.Json;

public static class JobService
{
    public static List<BackupJob> BackupJobs { get; } = [];
    public const int BackupJobLimit = 5;
    
    private const string BackupJobsFile = ".easysave/backup-jobs.json";
    
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
        var options = new JsonSerializerOptions { WriteIndented = true };
        
        var json = JsonSerializer.Serialize(BackupJobs, options);
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
}