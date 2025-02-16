using EasySave.Helpers;
using EasySave.Models.Backups;

namespace EasySave.Services;

using Models;
using System.Text.Json;

public static class JobService
{
    public static List<BackupJob> BackupJobs { get; } = [];
    public const int BackupJobLimit = 5;

    public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private const string BackupJobsFile = ".easysave/jobs-list.json";
    
    
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
}