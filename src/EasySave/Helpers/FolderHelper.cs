using EasySave.Models;
using static EasySave.Services.JobService;

namespace EasySave.Helpers;

public static class FolderHelper
{
    public static string GetLastCompleteBackupFolder(string sourceFolder)
    {
        var lastCompleteBackupJob = LoadLastCompleteBackupJob(sourceFolder);
        if (lastCompleteBackupJob == null) return "";
        return lastCompleteBackupJob.DestinationFolder;
    }
}