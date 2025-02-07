using Logger.LogEntries;

namespace Logger.Transporters;

public class FileJsonTransporter(string logRepositoryPath) : Transporter
{
    public override void Write(ILogEntry logEntry)
    {
        var logFilePath = GetLogFilePath();
        var parentDir = Path.GetDirectoryName(logFilePath)!;
        Directory.CreateDirectory(parentDir);
        
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
    
    private string GetLogFilePath()
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logRepositoryPath, $"log_{date}.json");
    }

    private string ReformatPaths(string json)
    {
        return json.Replace("\\\\", "\\")+",\n";
    }
}