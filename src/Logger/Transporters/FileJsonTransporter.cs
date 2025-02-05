using System.Runtime.Serialization.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Logger.LogEntries;

namespace Logger.Transporters;

public class FileJsonTransporter(string logRepositoryPath) : Transporter
{
    public override void Write(ILogEntry logEntry)
    {
        string logFilePath = GetLogFilePath();
        if (!File.Exists(logFilePath))
        {
            File.CreateText(logFilePath);
        }
        
        string json = ReFormatPath(logEntry.ToJson());
        
        StreamWriter sw = File.AppendText(logFilePath);
        using (sw)
        {
            sw.Write(json);
        }
    }
    
    private string GetLogFilePath()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logRepositoryPath, $"log_{date}.json");
    }

    private string ReFormatPath(string json)
    {
        return json.Replace("\\\\", "\\")+",\n";
    }
}