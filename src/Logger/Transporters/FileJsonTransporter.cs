using System.Text.Json;
using Logger.Helpers;
using Logger.LogEntries;

namespace Logger.Transporters;

public class FileJsonTransporter(string logRepositoryPath) : Transporter
{
    public override void Write(ILogEntry logEntry)
    {
        var logFilePath = FileHelper.GetLogFilePath(logRepositoryPath, ".json");
        FileHelper.CreateLogDirectoryIfNotExists(logFilePath);
        
        if (!File.Exists(logFilePath))
        {
            var streamWriter = File.CreateText(logFilePath);
            streamWriter.Close();
        }
        
        var json = ReformatPaths(logEntry.ToJson());
        
        var sw = File.AppendText(logFilePath);
        using (sw)
        {
            sw.Write(json);
        }
        sw.Close();
    }

    private string ReformatPaths(string json)
    {
        return json.Replace("\\\\", "\\")+",\n";
    }
}