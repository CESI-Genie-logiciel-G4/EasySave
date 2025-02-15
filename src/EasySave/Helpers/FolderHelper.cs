using static EasySave.Services.HistoryService;

namespace EasySave.Helpers;

public static class FolderHelper
{
    public static string GetLastCompleteBackupFolder(string sourceFolder)
    {
        var lastCompleteBackupJob = GetLastCompleteBackupJob(sourceFolder);
        return lastCompleteBackupJob == null ? "" : lastCompleteBackupJob.DestinationFolder;
    }
}