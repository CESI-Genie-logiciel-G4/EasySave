﻿using System.Collections.ObjectModel;
using DynamicData;
using EasySave.Helpers;
using EasySave.Models.Backups;

namespace EasySave.Services;

using Models;
using System.Text.Json;

public static class JobService
{
    public static ObservableCollection<BackupJob> BackupJobs { get; } = [];
    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { WriteIndented = true };
    
    private const string BackupJobsFile = ".easysave/jobs-list.json";
    
    
    public static BackupJob AddBackupJob(string name, string source, string destination, BackupType type, bool encryption)
    {
        var newJob = new BackupJob(name, source, destination, type, encryption);
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