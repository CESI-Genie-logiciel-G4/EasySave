using System.Diagnostics;
using EasySave.Models;
using Logger.LogEntries;

namespace EasySave.Helpers;

public static class FileHelper
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
    
    public static void CreateAndWrite(string path, string content)
    {
        try
        {
            CreateParentDirectory(path);
            File.WriteAllText(path, content);   
        } catch (Exception e)
        {
            Console.WriteLine($"Impossible to write file: { e.Message }");
        }
    }
    
    public static void CreateParentDirectory(string path)
    {
        var parentDir = Path.GetDirectoryName(path);
        if (parentDir is null)
        {
            throw new Exception("Parent directory is null");
        }
        Directory.CreateDirectory(parentDir);
    }
}