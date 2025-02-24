using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Logger.Helpers;
using Logger.LogEntries;

namespace Logger.Transporters;

[method: JsonConstructor]
public class FileJsonTransporter (string logRepositoryPath) : Transporter
{
    public String LogRepositoryPath { get; set; } = logRepositoryPath;
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void Write(ILogEntry logEntry)
    {
        var logFilePath = FileHelper.GetLogFilePath(LogRepositoryPath, ".json");
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