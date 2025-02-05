using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logger.LogEntries;

public class GlobalLogEntry(
    string loggedOperation, 
    string message, 
    Dictionary<string, object>? metadata = null)
    : ILogEntry
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string LoggedOperation { get; set; } = loggedOperation;
    public string Message { get; set; } = message;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; set; } = metadata;

    public override string ToString() => $"[{Timestamp}] - {LoggedOperation} - {Message}";
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}