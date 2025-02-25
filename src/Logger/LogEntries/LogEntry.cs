using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Logger.LogEntries;

[XmlInclude(typeof(CopyFileLogEntryV2))]
[XmlInclude(typeof(CopyFileLogEntry))]
[XmlInclude(typeof(GlobalLogEntry))]
public abstract class LogEntry(string loggedOperation) : ILogEntry
{
    [JsonPropertyOrder(1)]
    public DateTime Timestamp { get; } = DateTime.Now;
    
    [JsonPropertyOrder(2)]
    public string LoggedOperation { get; set; } = loggedOperation;


    public override string ToString() => $" [{Timestamp}] - {LoggedOperation}";
    public virtual string ToJson() => JsonSerializer.Serialize(this, this.GetType(), new JsonSerializerOptions { WriteIndented = true });
}