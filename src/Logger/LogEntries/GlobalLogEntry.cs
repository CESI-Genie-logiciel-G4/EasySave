using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Logger.LogEntries;

public class GlobalLogEntry : LogEntry
{
    public GlobalLogEntry() : base("") {}
    public GlobalLogEntry(string loggedOperation, string message, Dictionary<string, object>? metadata = null) : base(loggedOperation)
    {
        Message = message;
        Metadata = metadata;
    }
    
    [JsonPropertyOrder(3)]
    public string Message { get; set; } = null!;

    [JsonPropertyOrder(4)] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [XmlIgnore]
    public Dictionary<string, object>? Metadata { get; set; }

    public override string ToString() => $"[{Timestamp}] - {LoggedOperation} - {Message}";
}