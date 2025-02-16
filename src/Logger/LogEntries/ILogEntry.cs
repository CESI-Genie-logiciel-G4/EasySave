using System.Text.Json;

namespace Logger.LogEntries;

public interface ILogEntry
{
    public DateTime Timestamp { get; }
    public string LoggedOperation { get; set; }
    
    string ToString();
    string ToJson();
}