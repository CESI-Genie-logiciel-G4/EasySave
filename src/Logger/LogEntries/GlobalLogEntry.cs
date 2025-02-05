using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logger.LogEntries;

public class GlobalLogEntry : ILogEntry
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string LoggedOperation { get; set; }
    public required string Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; set; }

    public override string ToString() => $"[{Timestamp}] - {LoggedOperation} - {Message}";
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}