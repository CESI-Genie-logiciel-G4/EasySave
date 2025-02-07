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
            StreamWriter streamWriter = File.CreateText(logFilePath);
            streamWriter.Close();
        }
        
        string json = ReformatPaths(logEntry.ToJson());
        
        StreamWriter sw = File.AppendText(logFilePath);
        using (sw)
        {
            sw.Write(json);
        }
        sw.Close();
    }
    
    private string GetLogFilePath()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logRepositoryPath, $"log_{date}.json");
    }

    private string ReformatPaths(string json)
    {
        return json.Replace("\\\\", "\\")+",\n";
    }
}