using System.Xml;

namespace Logger.Helpers;

static class FileHelper
{
    public static string GetLogFilePath(string logRepositoryPath, string extension)
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logRepositoryPath, $"log_{date}{extension}");
    }

    public static void CreateLogDirectoryIfNotExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}