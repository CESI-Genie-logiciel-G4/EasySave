using System.Diagnostics;
using EasySave.Models;
using Logger.LogEntries;

namespace EasySave.Helpers;

public class FileHelper
{
    public static void Copy(string sourceFile, string destinationFile, BackupJob job)
    {
        var watch = new Stopwatch();
        
        watch.Start();
        Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
        File.Copy(sourceFile, destinationFile, true);
        watch.Stop();
        Logger.Logger.GetInstance().Log(new CopyFileLogEntry(
            job.Name,
            sourceFile,
            destinationFile,
            new FileInfo(sourceFile).Length,
            watch.ElapsedMilliseconds
        )); 
    }
}